using WebSocketMockServer.WebSockets;

namespace WebSocketMockServer.Reactions;


/// <summary>
/// Base class for all server reactions on ws request
/// </summary>
public abstract class Reaction
{
    /// <summary>
    /// Reaction message.
    /// </summary>
    public string Result { get; }

    /// <summary>
    /// Sends message to WebSocket.
    /// </summary>
    /// <param name="ws">web socket for sending.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns></returns>
    public abstract Task SendMessageAsync(IWebSocketProxy ws, CancellationToken ct);

    /// <summary>
    /// Creates reaction.
    /// </summary>
    /// <param name="result">Reaction data.</param>
    /// <param name="logger">Logger.</param>
    /// <exception cref="ArgumentException">If result is not set.</exception>
    /// <exception cref="ArgumentNullException">If result or logger is null.</exception>
    public Reaction(string result, ILogger<Reaction> logger)
    {
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(result);

        if (string.IsNullOrWhiteSpace(result))
        {
            throw new ArgumentException("Result not set", nameof(result));
        }

        Result = result;

        _logger = logger;

        _logger.LogDebug("Reaction for {Result} created.", result);
    }

    protected readonly ILogger<Reaction> _logger;
}
