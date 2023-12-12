namespace WebSocketMockServer.Reactions;

/// <summary>
/// Contract for Reaction factory logic.
/// </summary>
public interface IReactionFactory
{
    /// <summary>
    /// Creates <see cref="Response"/>.
    /// </summary>
    /// <param name="data">Response message</param>
    public Reaction Create(string data);

    /// <summary>
    /// Creates <see cref="Notification"/>.
    /// </summary>
    /// <param name="data">Notification message.</param>
    /// <param name="delay">Delay in ms.</param>
    public Reaction Create(string data, TimeSpan delay);
}
