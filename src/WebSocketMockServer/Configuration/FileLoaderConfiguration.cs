using System;
using System.Collections.Generic;

namespace WebSocketMockServer.Configuration
{
    public class FileLoaderConfiguration
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

        public void Validate()
        {
            if (string.IsNullOrEmpty(Folder))
                throw new ArgumentNullException("Json folder is not set.", nameof(Folder));

            if (Mapping == null)
                throw new ArgumentNullException("Templates of the config are not set.", nameof(Mapping));

            foreach (var template in Mapping!)
            {
                if (string.IsNullOrWhiteSpace(template.File))
                    throw new ArgumentNullException("Template file path not set.", nameof(template.File));

                if (template.Responses == null)
                    throw new ArgumentNullException($"Template {template.File} responses not set.", nameof(template.Responses));

                foreach (var res in template.Responses)
                {
                    if (string.IsNullOrWhiteSpace(res.File))
                        throw new ArgumentNullException($"Template {template.File} response file path not set.", nameof(res.File));
                }
            }
        }
    }
}

