using WebSocketMockServer.Scheduling;
using WebSocketMockServer.WebSockets;

namespace WebSocketMockServer.Reactions;

/// <summary>
/// Response with delay
/// </summary>
public class Notification : Reaction
{
    /// <summary>
    /// Response delay in ms.
    /// </summary>
    public TimeSpan Delay { get; }

    /// <summary>
    /// Creates delayed response.
    /// </summary>
    /// <param name="result">Notification message.</param>
    /// <param name="delay">Delay in ms.</param>
    /// <param name="logger">Logger.</param>
    /// <exception cref="ArgumentNullException">Throws if result or
    /// logger is null.</exception>
    /// <exception cref="ArgumentException">Throws if result is not set or
    /// delay is incorrect.</exception>
    public Notification(string result,
                        TimeSpan delay,
                        IWorkSheduler sheduler,
                        ILogger<Reaction> logger) :
        base(result, logger)
    {
        if (delay == TimeSpan.Zero)
        {
            throw new ArgumentException("Delay should be positive", nameof(delay));
        }

        _sheduler = sheduler ?? throw new ArgumentNullException(nameof(sheduler));

        Delay = delay;
    }

    /// <inheritdoc/>
    public override Task SendMessageAsync(IWebSocketProxy webSocket,
                                          CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(webSocket);
        ct.ThrowIfCancellationRequested();

        _sheduler.Schedule(async () =>
        {
            using var _ = _logger.BeginScope("Response {Response}", Result);

            _logger.LogDebug("Prepare for sending in {Seconds} seconds...", Delay);

            await Task.Delay(Delay).ConfigureAwait(false);

            _logger.LogDebug("Sending...");

            try
            {
                await webSocket.SendMessageAsync(Result, ct).ConfigureAwait(false);

                _logger.LogDebug("Has beed sended...");
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {

                _logger.LogError(ex, "Error on sending response");
            }

        }, ct);

        return Task.CompletedTask;
    }

    private readonly IWorkSheduler _sheduler;
}
