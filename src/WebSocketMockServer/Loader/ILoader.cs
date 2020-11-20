using System.Collections.Generic;

using WebSocketMockServer.Templates;

namespace WebSocketMockServer.Loader
{
    public interface ILoader
    {
        public IReadOnlyDictionary<string, MockTemplate> Load();
    }
}
