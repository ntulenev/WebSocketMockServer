using System;
using System.Collections.Generic;
using System.IO;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using WebSocketMockServer.Configuration;
using WebSocketMockServer.Templates;

namespace WebSocketMockServer.Loader
{
    public class FileLoader : ILoader
    {
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

        public IReadOnlyDictionary<string, MockTemplate> Load()
        {
            var templates = new Dictionary<string, MockTemplate>();

            foreach (var template in _config.Mapping!)
            {

                _logger?.LogInformation("Reading request from {filename}", template.File);

                var keyFileName = Path.Combine(_hostingEnvironment.ContentRootPath, _config.Folder!, template.File!);

                var keyText = File.ReadAllText(keyFileName);

                var responses = new List<Response>();


                foreach (var res in template.Responses!)
                {

                    _logger?.LogInformation("Reading response from {response} with delay {delay} ms", res.File, res.Delay);

                    var keyResFileName = Path.Combine(_hostingEnvironment.ContentRootPath, _config.Folder!, res.File!);

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

                templates.Add(keyText, new MockTemplate(keyText, responses));
            }

            return templates;
        }

        private readonly FileLoaderConfiguration _config;

        private readonly IWebHostEnvironment _hostingEnvironment;

        private readonly ILogger<FileLoader>? _logger;
    }
}
