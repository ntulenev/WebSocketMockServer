namespace WebSocketMockServer.IO;

/// <summary>
/// Wrapper for <see cref="File"/> and <see cref="Path"/> logic.
/// </summary>
public class FileUtility : IFileUtility
{
    /// <summary>
    /// Creates <see cref="FileUtility"/>.
    /// </summary>
    /// <param name="hostingEnvironment">Hosting environment.</param>
    /// <param name="logger">Logger.</param>
    /// <exception cref="ArgumentNullException">If one of params is null.</exception>
    public FileUtility(IWebHostEnvironment hostingEnvironment,
                       ILogger<FileUtility> logger)
    {
        _hostingEnvironment = hostingEnvironment ?? throw new ArgumentNullException(nameof(hostingEnvironment));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException">If folder or name is null</exception>
    public async Task<string> ReadFileAsync(string folder, string name, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(folder);
        ArgumentNullException.ThrowIfNull(name);

        var path = Path.Combine(_hostingEnvironment.ContentRootPath, folder, name);

        _logger.LogDebug("Loading file {File}", path);

        var result = await File.ReadAllTextAsync(path, ct).ConfigureAwait(false);

        _logger.LogDebug("File loaded {File} - {Result}.", path, result);

        return result;
    }

    private readonly IWebHostEnvironment _hostingEnvironment;
    private readonly ILogger<FileUtility> _logger;
}
