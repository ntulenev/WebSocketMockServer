namespace WebSocketMockServer.Reactions
{
    /// <summary>
    /// Factory for creating <see cref="Notification"/> and <see cref="Response"/>.
    /// </summary>
    public class ReactionFactory : IReactionFactory
    {
        /// <summary>
        /// Creates <see cref="ReactionFactory"/>.
        /// </summary>
        /// <param name="logger">Logger.</param>
        /// <exception cref="ArgumentNullException">Throws is logger is null.</exception>
        public ReactionFactory(ILogger<Reaction> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Creates <see cref="Response"/>.
        /// </summary>
        /// <param name="data">Response message</param>
        public Reaction Create(string data) => new Response(data, _logger);

        /// <summary>
        /// Creates <see cref="Notification"/>.
        /// </summary>
        /// <param name="data">Notification message.</param>
        /// <param name="delay">Delay in ms.</param>
        public Reaction Create(string data, int delay) => new Notification(data, delay, _logger);

        private readonly ILogger<Reaction> _logger;
    }
}
