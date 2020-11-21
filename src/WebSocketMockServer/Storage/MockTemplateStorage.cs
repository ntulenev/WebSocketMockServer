using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using Microsoft.Extensions.Logging;

using WebSocketMockServer.Loader;
using WebSocketMockServer.Models;

namespace WebSocketMockServer.Storage
{
    public class MockTemplateStorage : IMockTemplateStorage
    {
        public MockTemplateStorage(ILoader loader, ILogger<MockTemplateStorage>? logger)
        {

            if (loader == null)
                throw new ArgumentNullException(nameof(loader));

            try
            {
                _templates = loader.GetLoadedData();
            }
            catch (InvalidOperationException ex)
            {
                logger?.LogWarning(ex, "No templates load in storage");
                _templates = new Dictionary<string, MockTemplate>();
            }
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
