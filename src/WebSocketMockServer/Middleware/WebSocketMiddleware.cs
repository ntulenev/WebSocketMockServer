using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using WebSocketMockServer.Storage;
using WebSocketMockServer.Helpers;
using WebSocketMockServer.Models;

using WebSocketMockServer.WebSockets;

namespace WebSocketMockServer.Middleware
{
    /// <summary>
    /// Custom middleware that provides web sockets operations
    /// </summary>
    public class WebSocketMiddleware
    {
        /// <summary>
        /// Creates web sockets middleware
        /// </summary>
        public WebSocketMiddleware(RequestDelegate next,
                                   ILogger<WebSocketMiddleware>? logger,
                                   IHostApplicationLifetime hostApplicationLifetime,
                                   IMockTemplateStorage storage,
                                   ILoggerFactory? loggerFactory)
        {
            _next = next;
            _hostApplicationLifetime = hostApplicationLifetime;
            _storage = storage ?? throw new ArgumentNullException(nameof(storage));
            _loggerFactory = loggerFactory;
            _logger = logger;
        }

        private async Task ProcessRequestAsync(MockTemplate mockTemplate, IWebSocketProxy webSocket)
        {
            foreach (var reaction in mockTemplate.Reactions)
            {
                await reaction.SendMessage(webSocket, _hostApplicationLifetime.ApplicationStopping);
            }
        }

        private string ConvertBytesAsJsonString(byte[] bytes)
        {
            var request = Encoding.UTF8.GetString(bytes);

            if (!string.IsNullOrWhiteSpace(request))
            {
                try
                {
                    request = request.ReconvertWithJson();
                }
                catch
                {
                    // Unable to parse - just get original text  
                }
            }

            return request;
        }

        /// <summary>
        /// Handles user request
        /// </summary>
        public async Task Invoke(HttpContext httpContext)
        {

            if (httpContext.Request.Path == DEFAULT_PATH)
            {
                if (httpContext.WebSockets.IsWebSocketRequest)
                {
                    try
                    {
                        using WebSocket webSocket = await httpContext.WebSockets.AcceptWebSocketAsync();
                        var wsProxy = WebSocketProxy.Create(webSocket, _loggerFactory);

                        var adapter = new WebSocketsPipelinesAdapter(wsProxy, _hostApplicationLifetime.ApplicationStopping);

                        //Method for processing data
                        async Task ReadDataAsync()
                        {
                            try
                            {
                                await foreach (var bytes in adapter.ReadDataAsync())
                                {
                                    var request = ConvertBytesAsJsonString(bytes);

                                    _logger?.LogInformation("Get from client - {request}", request);

                                    if (_storage.TryGetTemplate(request, out var mockTemplate))
                                    {
                                        await ProcessRequestAsync(mockTemplate, wsProxy).ConfigureAwait(false);
                                    }
                                    else
                                    {
                                        _logger?.LogWarning("No predefiened response - closing socket");
                                        await wsProxy.CloseAsync(_hostApplicationLifetime.ApplicationStopping).ConfigureAwait(false);
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
                    catch (OperationCanceledException)
                    {
                        // Skip
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
        private readonly ILoggerFactory? _loggerFactory;

        private const string DEFAULT_PATH = "/ws";
    }
}
