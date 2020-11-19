using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebSocketMockServer.Configuration
{
    public class MockTemplatesConfiguration
    {
        public class ResponseTemplate
        {
            public string? Text { get; set; }

            public int? Delay { get; set; }
        }

        public Dictionary<string, IEnumerable<ResponseTemplate>>? Templates { get; set; }
    }
}
