using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using WebSocketMockServer.Templates;

namespace WebSocketMockServer.Storage
{
    public class PredefinedMockTemplateStorage : IMockTemplateStorage
    {
        public PredefinedMockTemplateStorage()
        {
            _templates = new Dictionary<string, MockTemplate>();
            AddTemplate(new MockTemplate("RequestA", new[]
            {
                 new Response("RequestA-Response1"),
                 new Response("RequestA-Response2")
            }));
            AddTemplate(new MockTemplate("RequestB", new[]
{
                 new Response("RequestB-Response1"),
                 new Response("RequestB-Response2")
            }));
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
