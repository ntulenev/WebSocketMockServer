using WebSocketMockServer.WebSockets;

namespace WebSocketMockServer.Reactions
{
    /// <summary>
    /// Response model.
    /// </summary>
    public class Response : Reaction
    {
        /// <summary>
        /// Creates response.
        /// </summary>
        /// <param name="result">Reaction data.</param>
        /// <param name="logger">Logger.</param>
        /// <exception cref="ArgumentNullException">Throws if result or logger is null.</exception>
        /// <exception cref="ArgumentException">Throws if result is not set.</exception>
        public Response(string result, ILogger<Reaction> logger) : base(result, logger)
        {
        }

        /// <inheritdoc/>
        public async override Task SendMessageAsync(IWebSocketProxy webSocket, CancellationToken ct)
        {
            ArgumentNullException.ThrowIfNull(webSocket);

            using var _ = _logger.BeginScope("Response {Response}", Result);

            _logger.LogDebug("Sending...");

            await webSocket.SendMessageAsync(Result, ct).ConfigureAwait(false);

            _logger.LogDebug("Has beed sended");
        }
    }
}
