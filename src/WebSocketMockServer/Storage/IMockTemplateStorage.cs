using System.Diagnostics.CodeAnalysis;

using WebSocketMockServer.Models;

namespace WebSocketMockServer.Storage
{
    /// <summary>
    /// Interface that provide logic to find teplates by key
    /// </summary>
    public interface IMockTemplateStorage
    {
        /// <summary>
        /// Trying to get template by key if any.
        /// </summary>
        public bool TryGetTemplate(string key, [NotNullWhen(true)] out MockTemplate? result);
    }
}
