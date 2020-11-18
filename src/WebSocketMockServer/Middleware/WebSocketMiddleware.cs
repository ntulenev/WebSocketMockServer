using SimpleWsApp;
using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;

using WebSocketMockServer.Storage;

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

        public async Task Invoke(HttpContext httpContext)
        {
            if (httpContext.Request.Path == "/ws")
            {
                if (httpContext.WebSockets.IsWebSocketRequest)
                {
                    using WebSocket webSocket = await httpContext.WebSockets.AcceptWebSocketAsync();

                    //Add adapter with minimumBufferSize
                    var adapter = new WebSocketsPipelinesAdapter(webSocket, _hostApplicationLifetime.ApplicationStopping);

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
                                        Console.WriteLine($"Send to client - {response.Result}");
                                        var data = Encoding.UTF8.GetBytes(response.Result);
                                        await webSocket.SendAsync(data.AsMemory(), WebSocketMessageType.Text, true, _hostApplicationLifetime.ApplicationStopping);
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("No predefiened response");
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
        private IMockTemplateStorage _storage;
        private IHostApplicationLifetime _hostApplicationLifetime;
    }
}
