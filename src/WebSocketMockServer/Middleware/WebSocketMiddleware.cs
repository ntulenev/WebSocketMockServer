using SimpleWsApp;
using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;

using WebSocketMockServer.Storage;

using Nito.AsyncEx;


namespace WebSocketMockServer.Middleware
{
    public class WebSocketMiddleware
    {
        public WebSocketMiddleware(RequestDelegate next, IHostApplicationLifetime hostApplicationLifetime, IMockTemplateStorage storage)
        {
            _next = next;
            _hostApplicationLifetime = hostApplicationLifetime;
            _storage = storage ?? throw new ArgumentNullException(nameof(storage));
        }

        private async Task SendMessage(WebSocket webSocket, string msg, CancellationToken ct)
        {

            var data = Encoding.UTF8.GetBytes(msg);

            using (await _socketSendGuard.LockAsync())
            {
                if (webSocket.State == WebSocketState.Open)
                {
                    Console.WriteLine($"{DateTime.UtcNow} Send to client - {msg}");
                    await webSocket.SendAsync(data.AsMemory(), WebSocketMessageType.Text, true, ct);
                }
                else
                {
                    Console.WriteLine($"{DateTime.UtcNow} Unable to send - {msg}. Socket is closed.");
                }
            }
        }

        public async Task Invoke(HttpContext httpContext)
        {
            if (httpContext.Request.Path == "/ws")
            {
                if (httpContext.WebSockets.IsWebSocketRequest)
                {
                    using WebSocket webSocket = await httpContext.WebSockets.AcceptWebSocketAsync();

                    //Add adapter with minimumBufferSize
                    var adapter = new WebSocketsPipelinesAdapter(webSocket, _socketReadGuard, 512, _hostApplicationLifetime.ApplicationStopping);

                    //Method for processing data
                    async Task ReadDataAsync()
                    {
                        try
                        {
                            await foreach (var bytes in adapter.ReadDataAsync())
                            {
                                var requst = Encoding.UTF8.GetString(bytes);
                                Console.WriteLine($"Get from client - {requst}");

                                if (_storage.TryGetTemplate(requst, out var mockTemplate))
                                {
                                    foreach (var response in mockTemplate.Responses)
                                    {
                                        if (response.IsNotification)
                                        {
                                            //TODO Add continuation check
                                            _ = Task.Run(async () =>
                                            {
                                                await Task.Delay(response.Delay).ConfigureAwait(false);
                                                await SendMessage(webSocket, response.Result, _hostApplicationLifetime.ApplicationStopping).ConfigureAwait(false);
                                            });
                                        }
                                        else
                                        {
                                            await SendMessage(webSocket, response.Result, _hostApplicationLifetime.ApplicationStopping).ConfigureAwait(false);
                                        }
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("No predefiened response");
                                    using (await _socketSendGuard.LockAsync())
                                    {
                                        using (await _socketReadGuard.LockAsync())
                                        {
                                            try
                                            {
                                                await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "No predefiened response", _hostApplicationLifetime.ApplicationStopping).ConfigureAwait(false);
                                            }
                                            catch (Exception ex)
                                            {
                                                Console.WriteLine(ex);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        catch (OperationCanceledException)
                        {
                            // Skip
                        }
                    }

                    await Task.WhenAll(adapter.StartAsync(), ReadDataAsync());
                }
                else
                {
                    httpContext.Response.StatusCode = 426;
                }
            }
            else
            {
                await _next(httpContext).ConfigureAwait(false);
            }
        }

        private readonly RequestDelegate _next;
        private readonly IMockTemplateStorage _storage;
        private readonly IHostApplicationLifetime _hostApplicationLifetime;

        private readonly AsyncLock _socketSendGuard = new AsyncLock();
        private readonly AsyncLock _socketReadGuard = new AsyncLock();
    }
}
