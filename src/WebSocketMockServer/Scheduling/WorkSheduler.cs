namespace WebSocketMockServer.Scheduling
{
    public class WorkSheduler : IWorkSheduler
    {
        public void Schedule(Func<Task> work, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(work);

            cancellationToken.ThrowIfCancellationRequested();

            _ = Task.Run(work, cancellationToken);
        }
    }
}
