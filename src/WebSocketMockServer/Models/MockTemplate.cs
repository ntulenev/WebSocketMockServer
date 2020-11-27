using System;
using System.Collections.Generic;
using System.Linq;

namespace WebSocketMockServer.Models
{
    /// <summary>
    /// Mock template model.
    /// </summary>
    public class MockTemplate
    {
        /// <summary>
        /// Request text.
        /// </summary>
        public string Request { get; }

        /// <summary>
        /// Responses for request.
        /// </summary>
        public IEnumerable<Response> Responses { get; }

        /// <summary>
        /// Creates <see cref="MockTemplate"/>.
        /// </summary>
        /// <param name="request">Request text</param>
        /// <param name="responses">Responses</param>
        /// <exception cref="ArgumentNullException">Throws if request data or responses is null.</exception>
        /// <exception cref="ArgumentException">Throws if request data or responses is empty.</exception>
        public MockTemplate(string request, IEnumerable<Response> responses)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (String.IsNullOrWhiteSpace(request))
            {
                throw new ArgumentException("Request not set", nameof(request));
            }

            Request = request;

            if (responses == null)
            {
                throw new ArgumentNullException(nameof(responses));
            }

            Responses = responses.ToList(); // Materialize

            if (!Responses.Any())
            {
                throw new ArgumentException("Responses is empty", nameof(responses));
            }
        }
    }
}
