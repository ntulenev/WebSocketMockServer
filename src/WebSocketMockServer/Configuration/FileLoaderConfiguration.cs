using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace WebSocketMockServer.Configuration
{

    /// <summary>
    /// Service configuration that is expected by <see cref="Loader.FileLoader"/>.
    /// </summary>
    public class FileLoaderConfiguration
    {
        /// <summary>
        /// Request/Reactions data.
        /// </summary>
        public IEnumerable<RequestMappingTemplate>? Mapping { get; set; }

        /// <summary>
        /// Root folder for tempalte files.
        /// </summary>
        public string? Folder { get; set; }
    }
}

