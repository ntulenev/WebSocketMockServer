using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebSocketMockServer.WebSockets;

namespace WebSocketMockServer.Models
{
    public abstract class Reaction
    {
        /// <summary>
        /// Response text.
        /// </summary>
        public string Result { get; }

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

        public abstract Task SendMessage(IWebSocketProxy ws, CancellationToken ct);

        public static Reaction Create(string data)
        {
            return new Response(data);
        }

        public static Reaction Create(string data, int delay)
        {
            return new Notification(data, delay);
        }
    }
}
