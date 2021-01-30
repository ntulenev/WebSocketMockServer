using System.Threading.Tasks;

using FluentAssertions;

using Xunit;

namespace WebSocketMockServer.Tests
{
    public class ServiceTests : IClassFixture<ServiceWebAppFactory>
    {
        public ServiceTests(ServiceWebAppFactory factory)
        {
            _factory = factory;
        }

        [Fact(DisplayName = "Service health check test.")]
        [Trait("Category", "Integration")]
        public async Task HealthCheckAsync()
        {
            var httpClient = _factory.CreateClient();
            var response = await httpClient.GetStringAsync("/hc").ConfigureAwait(false);
            response.Should().Be("Healthy");
        }

        //TODO Run and process Test

        private readonly ServiceWebAppFactory _factory;
    }
}
