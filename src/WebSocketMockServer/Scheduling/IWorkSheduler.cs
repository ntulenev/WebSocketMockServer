namespace WebSocketMockServer.Scheduling
{
    public interface IWorkSheduler
    {
        public void Schedule(Func<Task> work, CancellationToken cancellationToken);
    }
}
