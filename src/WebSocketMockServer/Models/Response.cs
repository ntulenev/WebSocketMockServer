using System;
using System.Threading;
using System.Threading.Tasks;
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

        public override Task SendMessage(IWebSocketProxy webSocket, CancellationToken ct)
        {
            if (webSocket is null)
                throw new ArgumentNullException(nameof(webSocket));

            return webSocket.SendMessageAsync(Result, ct);
        }
    }
}
