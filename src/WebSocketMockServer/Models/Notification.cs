using System;
using System.Threading;
using System.Threading.Tasks;

using WebSocketMockServer.WebSockets;

namespace WebSocketMockServer.Models
{
    /// <summary>
    /// Response with delay
    /// </summary>
    public class Notification : Reaction
    {
        /// <summary>
        /// Response delay in ms.
        /// </summary>
        public int Delay { get; }

        /// <summary>
        /// Creates delayed response.
        /// </summary>
        /// <exception cref="ArgumentNullException">Throws if result is null.</exception>
        /// <exception cref="ArgumentException">Throws if result is not set or delay is incorrect.</exception>
        public Notification(string result, int delay) : base(result)
        {
            if (delay <= 0)
            {
                throw new ArgumentException("Delay should be positive", nameof(delay));
            }

            Delay = delay;
        }

        /// <inheritdoc/>
        public override Task SendMessageAsync(IWebSocketProxy webSocket, CancellationToken ct)
        {
            if (webSocket is null)
            {
                throw new ArgumentNullException(nameof(webSocket));
            }

            //TODO Create separate IDelayedExecutionPool that responsible all delayed tasks and handle all issues if any. 
            _ = Task.Run(async () =>
            {
                await Task.Delay(Delay).ConfigureAwait(false);
                await webSocket.SendMessageAsync(Result, ct).ConfigureAwait(false);
            }, ct);

            return Task.CompletedTask;
        }
    }
}
