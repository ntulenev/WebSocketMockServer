using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using WebSocketMockServer.Helpers;
using WebSocketMockServer.Storage;

namespace WebSocketMockServer.WebSockets
{
    /// <summary>
    /// WebSocket processing infrastructure
    /// </summary>
    public class WebSocketHandler : IWebSocketHandler
    {

        /// <summary>
        /// Creates <see cref="WebSocketHandler"/>.
        /// </summary>
        public WebSocketHandler(ILogger<WebSocketHandler>? logger,
                                   IMockTemplateStorage storage)
        {
            _storage = storage ?? throw new ArgumentNullException(nameof(storage));
            _logger = logger;
        }

        /// <inheritdoc/>
        public async Task HandleAsync(IWebSocketProxy wsProxy, CancellationToken ct)
        {
            var adapter = new WebSocketsPipelinesAdapter(wsProxy, ct);

            //Method for processing data
            async Task ReadDataAsync()
            {
                try
                {
                    await foreach (var bytes in adapter.ReadDataAsync())
                    {
                        var request = ConvertBytesAsJsonString(bytes);

                        _logger?.LogInformation("Get from client - {request}", request);

                        if (_storage.TryGetTemplate(request, out var mockTemplate))
                        {
                            await ProcessRequestAsync(mockTemplate, wsProxy, ct).ConfigureAwait(false);
                        }
                        else
                        {
                            _logger?.LogWarning("No predefiened response - closing socket");
                            await wsProxy.CloseAsync(ct).ConfigureAwait(false);
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    // Skip
                }
            }

            await Task.WhenAll(adapter.StartAsync(), ReadDataAsync());
        }

        private static async Task ProcessRequestAsync(MockTemplate mockTemplate, IWebSocketProxy webSocket, CancellationToken ct)
        {
            foreach (var reaction in mockTemplate.Reactions)
            {
                await reaction.SendMessage(webSocket, ct);
            }
        }

        private static string ConvertBytesAsJsonString(byte[] bytes)
        {
            var request = Encoding.UTF8.GetString(bytes);

            if (!string.IsNullOrWhiteSpace(request))
            {
                try
                {
                    request = request.ReconvertWithJson();
                }
                catch
                {
                    // Unable to parse - just get original text  
                }
            }

            return request;
        }

        private readonly IMockTemplateStorage _storage;
        private readonly ILogger<WebSocketHandler>? _logger;
    }
}
