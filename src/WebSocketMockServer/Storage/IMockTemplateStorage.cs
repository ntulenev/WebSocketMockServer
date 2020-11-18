using System.Diagnostics.CodeAnalysis;

using WebSocketMockServer.Templates;

namespace WebSocketMockServer.Storage
{
    public interface IMockTemplateStorage
    {
        public bool TryGetTemplate(string key, [NotNullWhen(true)] out MockTemplate? result);

        public void AddTemplate(MockTemplate template);
    }
}
