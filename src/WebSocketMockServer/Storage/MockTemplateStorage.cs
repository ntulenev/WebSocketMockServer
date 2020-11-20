using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using WebSocketMockServer.Loader;
using WebSocketMockServer.Templates;

namespace WebSocketMockServer.Storage
{
    public class MockTemplateStorage : IMockTemplateStorage
    {
        public MockTemplateStorage(ILoader loader)
        {

            if (loader == null)
                throw new ArgumentNullException(nameof(loader));

            _templates = loader.Load();
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

        private readonly IReadOnlyDictionary<string, MockTemplate> _templates;
    }
}
