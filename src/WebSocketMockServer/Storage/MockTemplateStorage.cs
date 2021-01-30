using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using Microsoft.Extensions.Logging;

using WebSocketMockServer.Loader;

namespace WebSocketMockServer.Storage
{
    /// <summary>
    /// Storage for request/reactions templates.
    /// </summary>
    public class MockTemplateStorage : IMockTemplateStorage
    {
        /// <summary>
        /// Creates <see cref="MockTemplateStorage"/>
        /// </summary>
        /// <exception cref="ArgumentNullException">Thows if loader is not set.</exception>
        /// <exception cref="InvalidOperationException">Thows if loader has no templates.</exception>
        public MockTemplateStorage(ILoader loader, ILogger<MockTemplateStorage>? logger)
        {

            if (loader == null)
            {
                throw new ArgumentNullException(nameof(loader));
            }

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

        ///<inheritdoc/>
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
