namespace WebSocketMockServer.Reactions
{
    public class ReactionFactory : IReactionFactory
    {
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
