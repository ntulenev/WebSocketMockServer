using WebSocketMockServer.WebSockets;

namespace WebSocketMockServer.Models
{
    /// <summary>
    /// Response model.
    /// </summary>
    public class Response : Reaction
    {
        /// <summary>
        /// Creates response.
        /// </summary>
        /// <exception cref="ArgumentNullException">Throws if result is null.</exception>
        /// <exception cref="ArgumentException">Throws if result is not set.</exception>
        public Response(string result) : base(result)
        {
        }

        /// <inheritdoc/>
        public override Task SendMessageAsync(IWebSocketProxy webSocket, CancellationToken ct) =>
            webSocket is null ? throw new ArgumentNullException(nameof(webSocket)) : webSocket.SendMessageAsync(Result, ct);
    }
}
