using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using WebSocketMockServer.Configuration;
using WebSocketMockServer.Templates;

namespace WebSocketMockServer.Storage
{
    public class MockTemplateStorage : IMockTemplateStorage
    {
        public MockTemplateStorage(IOptions<MockTemplatesConfiguration> config, IWebHostEnvironment _hostingEnvironment)
        {

            if (config == null)
                throw new ArgumentNullException(nameof(config));

            var configData = config.Value;

            if (configData == null)
                throw new ArgumentException("Value of the config is not set.", nameof(config));

            _templates = new Dictionary<string, MockTemplate>();

            if (configData.Templates == null)
                throw new ArgumentException("Templates of the config are not set.", nameof(config));

            foreach (var template in configData.Templates!)
            {
                if (string.IsNullOrWhiteSpace(template.File))
                    throw new ArgumentException("Template file path not set.", nameof(config));

                var keyFileName = Path.Combine(_hostingEnvironment.ContentRootPath, template.File);

                var keyText = File.ReadAllText(keyFileName);

                var responses = new List<Response>();

                if (template.Responses == null)
                    throw new ArgumentException($"Template {template.File} responses not set.", nameof(config));

                foreach (var res in template.Responses)
                {
                    if (string.IsNullOrWhiteSpace(res.File))
                        throw new ArgumentException($"Template {template.File} response file path not set.", nameof(config));

                    var keyResFileName = Path.Combine(_hostingEnvironment.ContentRootPath, res.File);

                    var resText = File.ReadAllText(keyResFileName);

                    if (res.Delay.HasValue)
                    {
                        responses.Add(new Response(resText, res.Delay.Value));
                    }
                    else
                    {
                        responses.Add(new Response(resText));
                    }
                }

                AddTemplate(new MockTemplate(keyText, responses));


            }
        }

        public void AddTemplate(MockTemplate template)
        {
            _templates.Add(template.Request, template);
        }

        public bool TryGetTemplate(string key, [NotNullWhen(true)] out MockTemplate? result)
        {
            result = null;

            if (_templates.TryGetValue(key, out var value))
            {
                result = value;
                return true;
            }

            return false;
        }

        private readonly IDictionary<string, MockTemplate> _templates;
    }
}
