using System;
using System.Collections.Generic;
using System.Linq;

using WebSocketMockServer.Models;

namespace WebSocketMockServer.Storage
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
        public IEnumerable<Reaction> Reactions { get; }

        /// <summary>
        /// Creates <see cref="MockTemplate"/>.
        /// </summary>
        /// <param name="request">Request text</param>
        /// <param name="reactions">Responses</param>
        /// <exception cref="ArgumentNullException">Throws if request data or responses is null.</exception>
        /// <exception cref="ArgumentException">Throws if request data or responses is empty.</exception>
        public MockTemplate(string request, IEnumerable<Reaction> reactions)
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

            if (reactions == null)
            {
                throw new ArgumentNullException(nameof(reactions));
            }

            Reactions = reactions.ToList(); // Materialize

            if (!Reactions.Any())
            {
                throw new ArgumentException("Responses is empty", nameof(reactions));
            }
        }
    }
}
