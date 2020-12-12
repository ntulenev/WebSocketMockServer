using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using Nito.AsyncEx;

namespace WebSocketMockServer.WebSockets
{
    public class WebSocketProxy : IWebSocketProxy
    {
        public static WebSocketProxy Create(WebSocket ws, ILoggerFactory? factory)
        {
            return new WebSocketProxy(ws, factory?.CreateLogger<WebSocketProxy>());
        }
        private WebSocketProxy(WebSocket ws, ILogger<WebSocketProxy>? logger)
        {
            if (ws is null)
                throw new ArgumentNullException(nameof(ws));

            _webSocket = ws;
            _logger = logger;
        }

        public WebSocketState State => _webSocket.State;

        public async Task CloseAsync(CancellationToken ct)
        {
            using (await _socketWriteGuard.LockAsync())
            {
                using (await _socketReadGuard.LockAsync())
                {
                    try
                    {
                        if (State == WebSocketState.Open)
                            await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "No predefiened response", ct).ConfigureAwait(false);
                    }
                    catch (OperationCanceledException)
                    {
                        // Skip
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error on closing socket");
                    }
                }
            }
        }

        public ValueTask<ValueWebSocketReceiveResult> ReceiveAsync(Memory<byte> buffer, CancellationToken cancellationToken)
        => _webSocket.ReceiveAsync(buffer, cancellationToken);

        public async Task SendMessageAsync(string msg, CancellationToken ct)
        {
            var data = Encoding.UTF8.GetBytes(msg);

            using (await _socketWriteGuard.LockAsync())
            {
                if (State == WebSocketState.Open)
                {
                    _logger?.LogInformation("{Date} Send to client - {msg}", DateTime.UtcNow, msg);

                    try
                    {
                        await _webSocket.SendAsync(data.AsMemory(), WebSocketMessageType.Text, true, ct);
                    }
                    catch (OperationCanceledException)
                    {
                        // Skip
                    }
                    catch (Exception ex)
                    {
                        _logger?.LogError(ex, "{Date} Error on sending - {msg}.", DateTime.UtcNow, msg);
                    }
                }
                else
                {
                    _logger?.LogWarning("{Date} Unable to send - {msg}. Socket is closed.", DateTime.UtcNow, msg);
                }
            }
        }

        private readonly WebSocket _webSocket;

        private readonly AsyncLock _socketWriteGuard = new AsyncLock();
        private readonly AsyncLock _socketReadGuard = new AsyncLock();

        private readonly ILogger<WebSocketProxy>? _logger;
    }
}
