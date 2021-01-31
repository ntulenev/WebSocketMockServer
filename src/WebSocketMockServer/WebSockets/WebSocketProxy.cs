using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using Nito.AsyncEx;

namespace WebSocketMockServer.WebSockets
{
    /// <summary>
    /// Wrapper that gives abstraction of <see cref="WebSocket"/> for other layers.
    /// </summary>
    public class WebSocketProxy : IWebSocketProxy
    {
        /// <summary>
        /// Creates <see cref="WebSocketProxy"/>.
        /// </summary>
        public static IWebSocketProxy Create(WebSocket ws, ILoggerFactory? factory) =>
            new WebSocketProxy(ws, factory?.CreateLogger<WebSocketProxy>());

        /// <summary>
        ///  Creates <see cref="WebSocketProxy"/>.
        /// </summary>
        /// <param name="ws"></param>
        /// <param name="logger"></param>
        private WebSocketProxy(WebSocket ws, ILogger<WebSocketProxy>? logger)
        {
            if (ws is null)
            {
                throw new ArgumentNullException(nameof(ws));
            }

            _webSocket = ws;
            _logger = logger;
        }

        /// <inheritdoc/>
        public WebSocketState State => _webSocket.State;

        /// <inheritdoc/>
        public async Task CloseAsync(CancellationToken ct)
        {
            using (await _socketWriteGuard.LockAsync(ct).ConfigureAwait(false))
            {
                using (await _socketReadGuard.LockAsync(ct).ConfigureAwait(false))
                {
                    try
                    {
                        if (State == WebSocketState.Open)
                        {
                            await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "No predefiened response", ct).ConfigureAwait(false);
                        }
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

        /// <inheritdoc/>
        public async ValueTask<ValueWebSocketReceiveResult> ReceiveAsync(Memory<byte> buffer, CancellationToken ct)
        {
            using var _ = await _socketReadGuard.LockAsync(ct).ConfigureAwait(false);
            return await _webSocket.ReceiveAsync(buffer, ct).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async Task SendMessageAsync(string msg, CancellationToken ct)
        {
            var data = Encoding.UTF8.GetBytes(msg);

            using (await _socketWriteGuard.LockAsync(ct).ConfigureAwait(false))
            {
                if (State == WebSocketState.Open)
                {
                    _logger?.LogInformation("{Date} Send to client - {msg}", DateTime.UtcNow, msg);

                    try
                    {
                        await _webSocket.SendAsync(data.AsMemory(), WebSocketMessageType.Text, true, ct).ConfigureAwait(false);
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
