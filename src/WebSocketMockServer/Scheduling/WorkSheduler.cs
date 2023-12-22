namespace WebSocketMockServer.Scheduling;

/// <summary>
/// Creates <see cref="WorkSheduler"/>.
/// </summary>
/// <param name="logger">logger.</param>
/// <exception cref="ArgumentNullException">Throws is logger is null.</exception>
public sealed class WorkSheduler(ILogger<WorkSheduler> logger) : IWorkSheduler
{
    ///<inheritdoc/>
    ///<exception cref="ArgumentNullException">If work is null.</exception>
    ///<exception cref="OperationCanceledException">If token has cancellation.</exception>
    public void Schedule(Func<Task> work, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(work);
        cancellationToken.ThrowIfCancellationRequested();

        try
        {
            var t = Task.Run(work, cancellationToken);
            _ = t.ContinueWith(t =>
            {
                if (t.IsFaulted)
                {
                    _logger.LogError(t.Exception, "Error in sheduled work");
                }
            }, cancellationToken);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogError(ex, "Error on run sheduled work");
        }
    }

    private readonly ILogger<WorkSheduler> _logger = logger
                            ?? throw new ArgumentNullException(nameof(logger));
}
