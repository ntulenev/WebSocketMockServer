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

        public override Task SendMessage(IWebSocketProxy webSocket, CancellationToken ct)
        {
            //TODO Add track task to handle all not sended notifications.
            _ = Task.Run(async () =>
            {
                await Task.Delay(Delay).ConfigureAwait(false);
                await webSocket.SendMessageAsync(Result, ct).ConfigureAwait(false);
            });

            return Task.CompletedTask;
        }
    }
}
