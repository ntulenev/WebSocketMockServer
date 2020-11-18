using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using WebSocketMockServer.Configuration;
using WebSocketMockServer.Templates;

namespace WebSocketMockServer.Storage
{
    public class MockTemplateStorage : IMockTemplateStorage
    {
        public MockTemplateStorage(IOptions<MockTemplatesConfiguration> config)
        {
            var configData = config.Value!;

            _templates = new Dictionary<string, MockTemplate>();

            foreach (var template in configData.Templates!)
            {
                AddTemplate(new MockTemplate(template.Key, template.Value.Select(x => new Response(x))));
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
