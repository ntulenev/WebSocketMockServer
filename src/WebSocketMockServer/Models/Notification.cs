using System;


namespace WebSocketMockServer.Models
{
    /// <summary>
    /// Response with delay
    /// </summary>
    public class Notification : Response
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
                throw new ArgumentException("Deley should be positive", nameof(delay));
            }

            Delay = delay;
        }
    }
}
