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
    }
}
