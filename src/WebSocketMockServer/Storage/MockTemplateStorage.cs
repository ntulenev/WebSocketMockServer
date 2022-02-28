using System.Diagnostics.CodeAnalysis;

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
        public MockTemplateStorage(ILoader loader, ILogger<MockTemplateStorage> logger)
        {
            ArgumentNullException.ThrowIfNull(loader);

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            try
            {
                _templates = loader.GetLoadedData();
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "No templates load in storage");
                _templates = new Dictionary<string, MockTemplate>();
            }
        }

        ///<inheritdoc/>
        public bool TryGetTemplate(string key, [NotNullWhen(true)] out MockTemplate? result)
        {
            result = null;



            if (_templates.TryGetValue(key, out var value))
            {
                _logger.LogDebug("Getting template key {Key} - value {Value}", key, value);
                result = value;
                return true;
            }

            _logger.LogDebug("No data for key {Key}", key);

            return false;
        }

        private readonly IReadOnlyDictionary<string, MockTemplate> _templates;
        private readonly ILogger<MockTemplateStorage> _logger;
    }
}
