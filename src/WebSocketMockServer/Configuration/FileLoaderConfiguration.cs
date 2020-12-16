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

        /// <summary>
        /// Validates the current configuration.
        /// </summary>
        /// <exception cref="ConfigurationErrorsException">Throws is validation fails.</exception>
        public void Validate()
        {
            if (string.IsNullOrEmpty(Folder))
                throw new ConfigurationErrorsException("Root folder is not set.");

            if (Mapping == null)
                throw new ConfigurationErrorsException("Templates of the config are not set.");

            if (!Mapping.Any())
                throw new ConfigurationErrorsException("There is no any template in configuration.");

            foreach (var template in Mapping!)
            {
                if (string.IsNullOrWhiteSpace(template.File))
                    throw new ConfigurationErrorsException("Template file path not set.");

                if (template.Reactions == null)
                    throw new ConfigurationErrorsException($"Template {template.File} reactions not set.");

                if (!template.Reactions.Any())
                    throw new ConfigurationErrorsException("There is no any reactions in configuration.");

                foreach (var res in template.Reactions)
                {
                    if (string.IsNullOrWhiteSpace(res.File))
                        throw new ConfigurationErrorsException($"Template {template.File} reactions file path not set.");
                }
            }
        }
    }
}

