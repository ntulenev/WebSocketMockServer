using System;

namespace WebSocketMockServer.Models
{
    /// <summary>
    /// Response model.
    /// </summary>
    public class Response
    {
        /// <summary>
        /// Response text.
        /// </summary>
        public string Result { get; }

        /// <summary>
        /// Response delay.
        /// </summary>
        public int Delay => _delay!.Value;

        /// <summary>
        /// Checks if response has delay.
        /// </summary>
        public bool IsNotification => _delay.HasValue;

        /// <summary>
        /// Creates response.
        /// </summary>
        /// <exception cref="ArgumentNullException">Throws if result is null.</exception>
        /// <exception cref="ArgumentException">Throws if result is not set.</exception>
        public Response(string result)
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
        /// Creates delayed response.
        /// </summary>
        /// <exception cref="ArgumentNullException">Throws if result is null.</exception>
        /// <exception cref="ArgumentException">Throws if result is not set or delay is incorrect.</exception>
        public Response(string result, int delay)
        {
            if (result is null)
            {
                throw new ArgumentNullException(nameof(result));
            }

            if (String.IsNullOrWhiteSpace(result))
            {
                throw new ArgumentException("Result not set", nameof(result));
            }

            if (delay <= 0)
            {
                throw new ArgumentException("Deley should be positive", nameof(delay));
            }

            Result = result;
            _delay = delay;
        }

        private readonly int? _delay;
    }
}
