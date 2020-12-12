using System;
using System.Threading;
using System.Threading.Tasks;

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

        public abstract Task SendMessage(IWebSocketProxy ws, CancellationToken ct);

        /// <summary>
        /// Creates reaction.
        /// </summary>
        /// <param name="result">Reaction message</param>
        public Reaction(string result)
        {
            if (result is null)
            {
                throw new ArgumentNullException(nameof(result));
            }

            if (String.IsNullOrWhiteSpace(result))
            {
                throw new ArgumentException("Result not set", nameof(result));
            }

            Result = result;
        }

        /// <summary>
        /// Creates <see cref="Response"/>.
        /// </summary>
        /// <param name="data">Response message</param>
        public static Reaction Create(string data)
        {
            return new Response(data);
        }

        /// <summary>
        /// Creates <see cref="Notification"/>.
        /// </summary>
        /// <param name="data">Notification message.</param>
        /// <param name="delay">Delay in ms.</param>
        /// <returns></returns>
        public static Reaction Create(string data, int delay)
        {
            return new Notification(data, delay);
        }
    }
}
