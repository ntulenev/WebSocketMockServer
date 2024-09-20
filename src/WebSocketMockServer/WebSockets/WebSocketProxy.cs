using System.Net.WebSockets;
using System.Text;

using Nito.AsyncEx;

namespace WebSocketMockServer.WebSockets;

/// <summary>
/// Wrapper that gives abstraction of <see cref="WebSocket"/> for other layers.
/// </summary>
public sealed class WebSocketProxy : IWebSocketProxy
{
    /// <summary>
    /// Creates <see cref="WebSocketProxy"/>.
    /// </summary>
    public static IWebSocketProxy Create(WebSocket ws, ILoggerFactory factory)
    {
        ArgumentNullException.ThrowIfNull(ws);
        ArgumentNullException.ThrowIfNull(factory);

        return new WebSocketProxy(ws, factory.CreateLogger<WebSocketProxy>());
    }

    /// <summary>
    ///  Creates <see cref="WebSocketProxy"/>.
    /// </summary>
    /// <param name="ws"></param>
    /// <param name="logger"></param>
    private WebSocketProxy(WebSocket ws, ILogger<WebSocketProxy> logger)
    {
        ArgumentNullException.ThrowIfNull(ws);

        _webSocket = ws;
        _logger = logger;
    }

    /// <inheritdoc/>
    public WebSocketState State => _webSocket.State;

    /// <inheritdoc/>
    public async Task CloseAsync(CancellationToken ct)
    {
        ThrowIfDisposed();

        _socketClosingToken.Cancel();

        using (await _socketWriteGuard.LockAsync(ct).ConfigureAwait(false))
        {
            using (await _socketReadGuard.LockAsync(ct).ConfigureAwait(false))
            {
                try
                {
                    if (State == WebSocketState.Open)
                    {
                        await _webSocket.CloseAsync(
                            WebSocketCloseStatus.NormalClosure,
                            "No predefined response",
                            ct).ConfigureAwait(false);
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
    public async ValueTask<ValueWebSocketReceiveResult> ReceiveAsync(
                                    Memory<byte> buffer,
                                    CancellationToken ct)
    {
        ThrowIfDisposed();
        ct.ThrowIfCancellationRequested();

        using var source = CancellationTokenSource.CreateLinkedTokenSource(
                                                    ct,
                                                    _socketClosingToken.Token);

        using var _ = await _socketReadGuard.LockAsync(ct).ConfigureAwait(false);
        return await _webSocket.ReceiveAsync(buffer, source.Token).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task SendMessageAsync(string msg, CancellationToken ct)
    {
        ThrowIfDisposed();

        ArgumentNullException.ThrowIfNull(msg);

        using var source = CancellationTokenSource.CreateLinkedTokenSource(
                                                    ct,
                                                    _socketClosingToken.Token);

        var data = Encoding.UTF8.GetBytes(msg);

        using (await _socketWriteGuard.LockAsync(ct).ConfigureAwait(false))
        {
            if (State == WebSocketState.Open)
            {
                _logger.LogInformation("{Date} Send to client - {msg}", DateTime.UtcNow, msg);

                try
                {
                    await _webSocket.SendAsync(
                                        data.AsMemory(),
                                        WebSocketMessageType.Text,
                                        true,
                                        source.Token)
                                        .ConfigureAwait(false);
                }
                catch (OperationCanceledException)
                {
                    // Skip
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "{Date} Error on sending - {msg}.", DateTime.UtcNow, msg);
                }
            }
            else
            {
                _logger.LogWarning("{Date} Unable to send - {msg}. Socket is closed.", DateTime.UtcNow, msg);
            }
        }
    }


    private void ThrowIfDisposed()
    {
        if (_isDisposed)
        {
            throw new ObjectDisposedException($"Object is disposed {GetType().FullName}");
        }
    }

    ///<inheritdoc/>
    public void Dispose()
    {
        if (!_isDisposed)
        {
            _socketClosingToken.Dispose();
            _isDisposed = true;
        }
    }

    private readonly WebSocket _webSocket;
    private readonly AsyncLock _socketWriteGuard = new();
    private readonly AsyncLock _socketReadGuard = new();
    private readonly CancellationTokenSource _socketClosingToken = new();
    private readonly ILogger<WebSocketProxy> _logger;
    private bool _isDisposed;
}
