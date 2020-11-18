using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebSocketMockServer.Templates
{
    public class MockTemplate
    {
        public string Request { get; }

        public IEnumerable<Response> Responses { get; }

        public MockTemplate(string request, IEnumerable<Response> responses)
        {
            Request = request ?? throw new ArgumentNullException(request);

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
