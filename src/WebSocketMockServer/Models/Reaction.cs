using WebSocketMockServer.WebSockets;

namespace WebSocketMockServer.Models
{

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
        /// <param name="ct">Cancellatin token.</param>
        /// <returns></returns>
        public abstract Task SendMessageAsync(IWebSocketProxy ws, CancellationToken ct);

        /// <summary>
        /// Creates reaction.
        /// </summary>
        /// <param name="result">Reaction message</param>
        public Reaction(string result, ILogger<Reaction> logger)
        {
            ArgumentNullException.ThrowIfNull(result);

            if (string.IsNullOrWhiteSpace(result))
            {
                throw new ArgumentException("Result not set", nameof(result));
            }

            Result = result;

            _logger = logger;

            _logger.LogDebug("Reaction for {Result} created.", result);
        }

        /// <summary>
        /// Creates <see cref="Response"/>.
        /// </summary>
        /// <param name="data">Response message</param>
        public static Reaction Create(string data, ILogger<Reaction> logger) => new Response(data, logger);

        /// <summary>
        /// Creates <see cref="Notification"/>.
        /// </summary>
        /// <param name="data">Notification message.</param>
        /// <param name="delay">Delay in ms.</param>
        /// <returns></returns>
        public static Reaction Create(string data, int delay, ILogger<Reaction> logger) => new Notification(data, delay, logger);

        protected readonly ILogger<Reaction> _logger;
    }
}
