using System.Collections.Generic;

namespace WebSocketMockServer.Configuration
{
    /// <summary>
    /// Sub-configuration class for <see cref="FileLoaderConfiguration"/> that represents Request mapping.
    /// </summary>
    public class RequestMappingTemplate
    {
        /// <summary>
        /// Request file name.
        /// </summary>
        public string? File { get; set; }

        /// <summary>
        /// Responses for request
        /// </summary>
        public IEnumerable<ResponseMappingTemplate>? Responses { get; set; }
    }
}
