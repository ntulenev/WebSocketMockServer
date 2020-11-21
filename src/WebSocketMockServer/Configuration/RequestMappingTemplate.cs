using System.Collections.Generic;

namespace WebSocketMockServer.Configuration
{
    public class RequestMappingTemplate
    {
        public string? File { get; set; }

        public IEnumerable<ResponseMappingTemplate>? Responses { get; set; }
    }
}
