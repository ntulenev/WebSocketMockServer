using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebSocketMockServer.Configuration
{
    public class MockTemplatesConfiguration
    {
        public class Template
        {
            public string? File { get; set; }

            public IEnumerable<ResponseTemplate>? Responses { get; set; }
        }

        public class ResponseTemplate
        {
            public string? File { get; set; }

            public int? Delay { get; set; }
        }

        public IEnumerable<Template>? Mapping { get; set; }

        public string? Folder { get; set; }
    }
}
