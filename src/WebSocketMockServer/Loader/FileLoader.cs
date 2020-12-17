using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using WebSocketMockServer.Configuration;
using WebSocketMockServer.Helpers;
using WebSocketMockServer.Models;
using WebSocketMockServer.Storage;

namespace WebSocketMockServer.Loader
{
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
        public FileLoader(IOptions<FileLoaderConfiguration> config, ILogger<FileLoader>? logger, IWebHostEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment ?? throw new ArgumentNullException(nameof(hostingEnvironment));

            if (config == null)
                throw new ArgumentNullException(nameof(config));

            var configData = config.Value;

            if (configData == null)
                throw new ArgumentException("Value of the config is not set.", nameof(config));

            configData.Validate();

            _config = configData;

            _logger = logger;
        }


        private string GetFilePath(string name)
            => Path.Combine(_hostingEnvironment.ContentRootPath, _config.Folder!, name);

        private async Task<string> GetFileContentAsync(string fileName, CancellationToken ct)
        {
            var requestText = await File.ReadAllTextAsync(GetFilePath(fileName), ct);
            return requestText.ReconvertWithJson();
        }

        ///<inheritdoc/>
        public IReadOnlyDictionary<string, MockTemplate> GetLoadedData() => _data ?? throw new InvalidOperationException("Data not loaded");

        ///<inheritdoc/>
        public async Task LoadAsync(CancellationToken ct)
        {
            var templates = new Dictionary<string, MockTemplate>();

            foreach (var template in _config.Mapping!)
            {

                _logger?.LogInformation("Reading request from {filename}", template.File);

                var requestText = await GetFileContentAsync(template.File!, ct);

                var reactions = new List<Reaction>();

                foreach (var res in template.Reactions!)
                {
                    if (res.Delay.HasValue)
                    {
                        _logger?.LogInformation("Reading notification from {response} with delay {delay} ms", res.File, res.Delay);
                        var reactionText = await GetFileContentAsync(res.File!, ct);
                        reactions.Add(Reaction.Create(reactionText, res.Delay.Value));
                    }
                    else
                    {
                        _logger?.LogInformation("Reading response from {response}", res.File, res.Delay);
                        var reactionText = await GetFileContentAsync(res.File!, ct);
                        reactions.Add(Reaction.Create(reactionText));
                    }
                }

                templates.Add(requestText, new MockTemplate(requestText, reactions));
            }

            _data = templates;
        }

        private readonly FileLoaderConfiguration _config;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly ILogger<FileLoader>? _logger;
        private IReadOnlyDictionary<string, MockTemplate> _data = null!;
    }
}
