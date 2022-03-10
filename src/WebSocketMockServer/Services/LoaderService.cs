using WebSocketMockServer.Loader;

namespace WebSocketMockServer.Services
{
    /// <summary>
    /// Hosted service that loads templates before web service starts.
    /// </summary>
    public class LoaderService : IHostedService
    {
        public LoaderService(ILogger<LoaderService> logger,
                             IHostApplicationLifetime hostApplicationLifetime,
                             Loader.ILoader loader
                                    )
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _hostApplicationLifetime = hostApplicationLifetime ?? throw new ArgumentNullException(nameof(hostApplicationLifetime));
            _loader = loader ?? throw new ArgumentNullException(nameof(loader));
        }

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

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        private readonly ILogger<LoaderService> _logger;
        private readonly IHostApplicationLifetime _hostApplicationLifetime;
        private readonly ILoader _loader;
    }
}
