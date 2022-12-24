namespace WebSocketMockServer.Reactions;

/// <summary>
/// Factory for creating <see cref="Notification"/> and <see cref="Response"/>.
/// </summary>
public class ReactionFactory : IReactionFactory
{

    /// <summary>
    /// Creates <see cref="ReactionFactory"/>.
    /// </summary>
    /// <param name="responseFactory">Factory delegate for <see cref="Response"/>.</param>
    /// <param name="notificationFactory">Factory deletega for <see cref="Notification"/>.</param>
    /// <exception cref="ArgumentNullException">Throws is factory delegates are null.</exception>
    public ReactionFactory(Func<string, Response> responseFactory,
                           Func<string, int, Notification> notificationFactory)
    {
        _responseFactory = responseFactory ?? throw new ArgumentNullException(nameof(responseFactory));
        _notificationFactory = notificationFactory ?? throw new ArgumentNullException(nameof(notificationFactory));
    }

    /// <summary>
    /// Creates <see cref="Response"/>.
    /// </summary>
    /// <param name="data">Response message</param>
    public Reaction Create(string data) => _responseFactory(data);

    /// <summary>
    /// Creates <see cref="Notification"/>.
    /// </summary>
    /// <param name="data">Notification message.</param>
    /// <param name="delay">Delay in ms.</param>
    public Reaction Create(string data, int delay) => _notificationFactory(data, delay);

    private readonly Func<string, Response> _responseFactory;
    private readonly Func<string, int, Notification> _notificationFactory;
}
