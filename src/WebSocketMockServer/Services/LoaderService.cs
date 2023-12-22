using WebSocketMockServer.Loader;

namespace WebSocketMockServer.Services;

/// <summary>
/// Represents a hosted service that loads templates before the web service starts.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="LoaderService"/> class.
/// </remarks>
/// <param name="logger">The logger to use for logging information.</param>
/// <param name="hostApplicationLifetime">The lifetime object for the hosted application.</param>
/// <param name="loader">The loader to use for loading templates.</param>
/// <exception cref="ArgumentNullException">Thrown when any argument is null.</exception>
public sealed class LoaderService(
                     ILogger<LoaderService> logger,
                     IHostApplicationLifetime hostApplicationLifetime,
                     ILoader loader) : IHostedService
{
    /// <summary>
    /// Triggered when the application host is ready to start the service.
    /// </summary>
    /// <param name="cancellationToken">Indicates that the start process has been aborted.</param>
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            await _loader.LoadAsync(cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogCritical(ex, "Error on load templates");
            _hostApplicationLifetime.StopApplication();
        }
    }

    /// <summary>
    /// Triggered when the application host is performing a graceful shutdown.
    /// </summary>
    /// <param name="cancellationToken">Indicates that the shutdown process should no longer be graceful.</param>
    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    private readonly ILogger<LoaderService> _logger = logger
                                            ?? throw new ArgumentNullException(nameof(logger));
    private readonly IHostApplicationLifetime _hostApplicationLifetime = hostApplicationLifetime
                                            ?? throw new ArgumentNullException(nameof(hostApplicationLifetime));
    private readonly ILoader _loader = loader
                                            ?? throw new ArgumentNullException(nameof(loader));

}
