using WebSocketMockServer.WebSockets;

namespace WebSocketMockServer.Middleware
{
    /// <summary>
    /// Custom middleware that provides web sockets operations
    /// </summary>
    public class CustomWebSocketMiddleware
    {
        /// <summary>
        /// Creates web sockets middleware
        /// </summary>
        public CustomWebSocketMiddleware(RequestDelegate next,
                                   ILogger<CustomWebSocketMiddleware>? logger,
                                   IHostApplicationLifetime hostApplicationLifetime,
                                   IWebSocketHandler handler,
                                   ILoggerFactory? loggerFactory
                                   )
        {
            _next = next;
            _hostApplicationLifetime = hostApplicationLifetime;
            _handler = handler ?? throw new ArgumentNullException(nameof(handler));
            _logger = logger;
            _loggerFactory = loggerFactory;
        }

        /// <summary>
        /// Handles user request
        /// </summary>
        public async Task InvokeAsync(HttpContext httpContext)
        {
            if (httpContext.Request.Path == DEFAULT_PATH)
            {
                if (httpContext.WebSockets.IsWebSocketRequest)
                {
                    try
                    {
                        using var webSocket = await httpContext.WebSockets.AcceptWebSocketAsync().ConfigureAwait(false);
                        using var wsProxy = WebSocketProxy.Create(webSocket, _loggerFactory);
                        await _handler.HandleAsync(wsProxy, _hostApplicationLifetime.ApplicationStopping).ConfigureAwait(false);
                    }
                    catch (OperationCanceledException)
                    {
                        // Skip
                    }
                    catch (Exception ex)
                    {
                        _logger?.LogError(ex, "Error on process ws");
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
        private readonly IWebSocketHandler _handler;
        private readonly IHostApplicationLifetime _hostApplicationLifetime;
        private readonly ILogger<CustomWebSocketMiddleware>? _logger;
        private readonly ILoggerFactory? _loggerFactory;
        private const string DEFAULT_PATH = "/ws";
    }
}
