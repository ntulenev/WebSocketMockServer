using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using WebSocketMockServer.Storage;

using Nito.AsyncEx;

namespace WebSocketMockServer.Middleware
{
    public class WebSocketMiddleware
    {
        public WebSocketMiddleware(RequestDelegate next, ILogger<WebSocketMiddleware>? logger, IHostApplicationLifetime hostApplicationLifetime, IMockTemplateStorage storage)
        {
            _next = next;
            _hostApplicationLifetime = hostApplicationLifetime;
            _storage = storage ?? throw new ArgumentNullException(nameof(storage));

            _logger = logger;
        }

        private async Task SendMessage(WebSocket webSocket, string msg, CancellationToken ct)
        {

            var data = Encoding.UTF8.GetBytes(msg);

            using (await _socketSendGuard.LockAsync())
            {
                if (webSocket.State == WebSocketState.Open)
                {
                    _logger?.LogInformation("{Date} Send to client - {msg}", DateTime.UtcNow, msg);

                    await webSocket.SendAsync(data.AsMemory(), WebSocketMessageType.Text, true, ct);
                }
                else
                {
                    _logger?.LogWarning("{Date} Unable to send - {msg}. Socket is closed.", DateTime.UtcNow, msg);
                }
            }
        }

        public async Task Invoke(HttpContext httpContext)
        {

            if (httpContext.Request.Path == "/ws")
            {
                if (httpContext.WebSockets.IsWebSocketRequest)
                {
                    try
                    {
                        using WebSocket webSocket = await httpContext.WebSockets.AcceptWebSocketAsync();

                        //Add adapter with minimumBufferSize
                        var adapter = new WebSocketsPipelinesAdapter(webSocket, _socketReadGuard, _hostApplicationLifetime.ApplicationStopping);

                        //Method for processing data
                        async Task ReadDataAsync()
                        {
                            try
                            {
                                await foreach (var bytes in adapter.ReadDataAsync())
                                {
                                    var requst = Encoding.UTF8.GetString(bytes);

                                    _logger?.LogInformation("Get from client - {requst}", requst);

                                    if (_storage.TryGetTemplate(requst, out var mockTemplate))
                                    {
                                        foreach (var response in mockTemplate.Responses)
                                        {
                                            if (response.IsNotification)
                                            {
                                                //TODO Add continuation fail check
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
                                        _logger?.LogWarning("No predefiened response - closing socket");

                                        using (await _socketSendGuard.LockAsync())
                                        {
                                            using (await _socketReadGuard.LockAsync())
                                            {
                                                try
                                                {
                                                    if (webSocket.State == WebSocketState.Open)
                                                        await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "No predefiened response", _hostApplicationLifetime.ApplicationStopping).ConfigureAwait(false);
                                                }
                                                catch (Exception ex)
                                                {
                                                    _logger.LogError(ex, "Error on closing socket");
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
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error on process ws");
                    }
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

        private readonly ILogger<WebSocketMiddleware>? _logger;

        private readonly AsyncLock _socketSendGuard = new AsyncLock();
        private readonly AsyncLock _socketReadGuard = new AsyncLock();
    }
}
