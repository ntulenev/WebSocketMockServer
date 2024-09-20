namespace WebSocketMockServer.Scheduling;

/// <summary>
/// Defines logic for scheduling some async logic to be done in future.
/// </summary>
public interface IWorkScheduler
{
    /// <summary>
    /// Schedules work.
    /// </summary>
    /// <param name="work">Work to be done.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public void Schedule(Func<Task> work, CancellationToken cancellationToken);
}
