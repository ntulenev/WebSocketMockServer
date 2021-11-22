using WebSocketMockServer.Storage;

namespace WebSocketMockServer.Loader
{
    /// <summary>
    /// An interface to load request/response templates.
    /// </summary>
    public interface ILoader
    {
        /// <summary>
        /// Returns a dictionary of request/response templates with key as request string.
        /// </summary>
        /// <returns></returns>
        public IReadOnlyDictionary<string, MockTemplate> GetLoadedData();

        /// <summary>
        /// Loads all request from storage.
        /// </summary>
        /// <param name="ct">Token for cancellation.</param>
        public Task LoadAsync(CancellationToken ct);
    }
}
