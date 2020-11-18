using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebSocketMockServer.Configuration
{
    public class MockTemplatesConfiguration
    {
        public Dictionary<string, IEnumerable<string>>? Templates { get; set; }
    }
}
