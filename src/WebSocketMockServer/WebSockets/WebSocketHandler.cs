using System.Text;

using WebSocketMockServer.Helpers;
using WebSocketMockServer.Storage;

namespace WebSocketMockServer.WebSockets;

/// <summary>
/// WebSocket processing infrastructure
/// </summary>
/// <param name="logger">The logger to use for logging information.</param>
/// <param name="storage">Mock templates storage.</param>
/// <exception cref="ArgumentNullException">Thrown when storage is null.</exception>
/// <remarks>
/// Creates <see cref="WebSocketHandler"/>.
/// </remarks>
public sealed class WebSocketHandler(
                           ILogger<WebSocketHandler> logger,
                           IMockTemplateStorage storage) : IWebSocketHandler
{
    /// <inheritdoc/>
    public async Task HandleAsync(IWebSocketProxy wsProxy, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(wsProxy);
        ct.ThrowIfCancellationRequested();

        var adapter = new WebSocketsPipelinesAdapter(wsProxy, ct);

        //Method for processing data
        async Task ReadDataAsync()
        {
            try
            {
                await foreach (var bytes in adapter.ReadDataAsync().ConfigureAwait(false))
                {
                    var request = ConvertBytesAsJsonString(bytes);

                    _logger.LogInformation("Get from client - {request}", request);

                    if (_storage.TryGetTemplate(request, out var mockTemplate))
                    {
                        await ProcessRequestAsync(mockTemplate, wsProxy, ct).ConfigureAwait(false);
                    }
                    else
                    {
                        _logger.LogWarning("No predefined response - closing socket");
                        await wsProxy.CloseAsync(ct).ConfigureAwait(false);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // Skip
            }
        }

        await Task.WhenAll(adapter.StartAsync(), ReadDataAsync()).ConfigureAwait(false);
    }

    private static async Task ProcessRequestAsync(
        MockTemplate mockTemplate,
        IWebSocketProxy webSocket,
        CancellationToken ct)
    {
        foreach (var reaction in mockTemplate.Reactions)
        {
            await reaction.SendMessageAsync(webSocket, ct).ConfigureAwait(false);
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

    private readonly IMockTemplateStorage _storage = storage
                                          ?? throw new ArgumentNullException(nameof(storage));
    private readonly ILogger _logger = logger
                                          ?? throw new ArgumentNullException(nameof(logger));
}
