using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using WebSocketMockServer.Templates;

namespace WebSocketMockServer.Loader
{
    public interface ILoader
    {
        public IReadOnlyDictionary<string, MockTemplate> GetLoadedData();

        public Task LoadAsync(CancellationToken ct);
    }
}
