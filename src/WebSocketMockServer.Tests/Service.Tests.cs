using System.Net.WebSockets;
using System.Text;

using FluentAssertions;

using Microsoft.AspNetCore.Mvc.Testing;

using Xunit;

namespace WebSocketMockServer.Tests;

public class ServiceTests : IClassFixture<WebApplicationFactory<Program>>
{
    public ServiceTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact(DisplayName = "Service health check test.")]
    [Trait("Category", "Integration")]
    public async Task HealthCheckAsync()
    {
        var httpClient = _factory.CreateClient();
        var response = await httpClient.GetStringAsync("/hc");
        response.Should().Be("Healthy");
    }

    [Fact(DisplayName = "Service works with messages properly.")]
    [Trait("Category", "Integration")]
    public async Task ServiceMainCaseWorksCorrectAsync()
    {
        var uri = new Uri("ws://localhost/ws");

        var cts = new CancellationTokenSource(50_000);

        var client = _factory.Server.CreateWebSocketClient();

        var webSocket = await client.ConnectAsync(uri, cts.Token);

        var request1 = @"{" +
                            "\"Request\": \"A\"" +
                        "}";

        var response1 = "{\r\n  \"Response\": \"A\"\r\n}";

        var response2 = "{\r\n  \"Notification\": \"B\"\r\n}";

        var encoded = Encoding.UTF8.GetBytes(request1);

        await webSocket.SendAsync(encoded, WebSocketMessageType.Text, true, cts.Token);

        var realResponse1 = await GetWSDataAsync(webSocket, cts.Token);

        realResponse1.Should().Be(response1);

        var realResponse2 = await GetWSDataAsync(webSocket, cts.Token);

        realResponse2.Should().Be(response2);

    }

    private static async Task<string> GetWSDataAsync(WebSocket webSocket, CancellationToken ct)
    {
        var bufferList = new List<byte>();
        while (webSocket.State == WebSocketState.Open)
        {
            var bufferResult = new byte[1024];
            var result = await webSocket.ReceiveAsync(bufferResult, ct).ConfigureAwait(false);

            bufferList.AddRange(bufferResult.Take(result.Count));

            if (result.EndOfMessage)
            {
                break;
            }
        }

        return Encoding.UTF8.GetString(bufferList.ToArray());
    }

    private readonly WebApplicationFactory<Program> _factory;
}
