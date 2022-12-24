using Microsoft.Extensions.Options;

using WebSocketMockServer.Configuration;
using WebSocketMockServer.Helpers;
using WebSocketMockServer.IO;
using WebSocketMockServer.Reactions;
using WebSocketMockServer.Storage;

namespace WebSocketMockServer.Loader;

/// <summary>
/// Implementation of <see cref="ILoader"/> that loads files from disk.
/// </summary>
public class FileLoader : ILoader
{
    /// <summary>
    /// Creates <see cref="FileLoader"/>.
    /// </summary>
    /// <param name="config">Loader configuration.</param>
    /// <param name="logger">Logger.</param>
    /// <param name="hostingEnvironment">Hosting environment.</param>
    public FileLoader(IOptions<FileLoaderConfiguration> config,
                      ILogger<FileLoader> logger,
                      IFileUtility fileUtility,
                      IReactionFactory reactionFactory)
    {
        ArgumentNullException.ThrowIfNull(config);

        var configData = config.Value;

        if (configData == null)
        {
            throw new ArgumentException("Value of the config is not set.", nameof(config));
        }

        _config = configData;

        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _fileUtility = fileUtility ?? throw new ArgumentNullException(nameof(fileUtility));
        _reactionFactory = reactionFactory ?? throw new ArgumentNullException(nameof(reactionFactory));
    }

    private async Task<string> GetFileContentAsync(string fileName, CancellationToken ct)
    {
        var requestText = await _fileUtility.ReadFileAsync(_config.Folder!, fileName, ct).ConfigureAwait(false);
        return requestText.ReconvertWithJson();
    }

    ///<inheritdoc/>
    public IReadOnlyDictionary<string, MockTemplate> GetLoadedData() => _data ?? throw new InvalidOperationException("Data not loaded");

    ///<inheritdoc/>
    public async Task LoadAsync(CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        var templates = new Dictionary<string, MockTemplate>();

        foreach (var template in _config.Mapping!)
        {
            _logger.LogInformation("Reading request from {filename}", template.File);

            var requestText = await GetFileContentAsync(template.File!, ct).ConfigureAwait(false);

            var reactions = new List<Reaction>();

            foreach (var res in template.Reactions!)
            {
                if (res.Delay.HasValue)
                {
                    _logger.LogInformation("Reading notification from {response} with delay {delay} ms", res.File, res.Delay);
                    var reactionText = await GetFileContentAsync(res.File!, ct).ConfigureAwait(false);
                    reactions.Add(_reactionFactory.Create(reactionText, res.Delay.Value));
                }
                else
                {
                    _logger.LogInformation("Reading response from {response} with delay {delay} ms", res.File, res.Delay);
                    var reactionText = await GetFileContentAsync(res.File!, ct).ConfigureAwait(false);
                    reactions.Add(_reactionFactory.Create(reactionText));
                }
            }

            templates.Add(requestText, new MockTemplate(requestText, reactions));
        }

        _data = templates;
    }

    private readonly FileLoaderConfiguration _config;
    private readonly ILogger<FileLoader> _logger;
    private readonly IFileUtility _fileUtility;
    private readonly IReactionFactory _reactionFactory;

    private IReadOnlyDictionary<string, MockTemplate> _data = null!;

}
