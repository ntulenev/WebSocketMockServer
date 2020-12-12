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


        ///<inheritdoc/>
        public IReadOnlyDictionary<string, MockTemplate> GetLoadedData() => _data ?? throw new InvalidOperationException("Data not loaded");

        ///<inheritdoc/>
        public async Task LoadAsync(CancellationToken ct)
        {
            var templates = new Dictionary<string, MockTemplate>();

            foreach (var template in _config.Mapping!)
            {

                _logger?.LogInformation("Reading request from {filename}", template.File);

                var keyFileName = Path.Combine(_hostingEnvironment.ContentRootPath, _config.Folder!, template.File!);

                var keyText = await File.ReadAllTextAsync(keyFileName, ct);

                keyText = keyText.ReconvertWithJson();

                var reactions = new List<Reaction>();

                foreach (var res in template.Responses!)
                {
                    if (res.Delay.HasValue)
                        _logger?.LogInformation("Reading response from {response} with delay {delay} ms", res.File, res.Delay);
                    else
                        _logger?.LogInformation("Reading response from {response}", res.File, res.Delay);

                    var keyResFileName = Path.Combine(_hostingEnvironment.ContentRootPath, _config.Folder!, res.File!);

                    var resText = await File.ReadAllTextAsync(keyResFileName, ct);

                    resText = resText.ReconvertWithJson();

                    if (res.Delay.HasValue)
                    {
                        reactions.Add(Reaction.Create(resText, res.Delay.Value));
                    }
                    else
                    {
                        reactions.Add(Reaction.Create(resText));
                    }
                }

                templates.Add(keyText, new MockTemplate(keyText, reactions));
            }

            _data = templates;
        }

        private readonly FileLoaderConfiguration _config;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly ILogger<FileLoader>? _logger;
        private IReadOnlyDictionary<string, MockTemplate> _data = null!;
    }
}
