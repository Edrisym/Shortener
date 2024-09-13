using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;


public class RateLimiterIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public RateLimiterIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Should_AllowRequests_UnderRateLimit()
    {
        for (int i = 0; i < 5; i++)
        {
            var response = await _client.GetAsync("/test");
            Assert.True(response.IsSuccessStatusCode);
        }
    }

    [Fact]
    public async Task Should_DenyRequests_OverRateLimit()
    {
        for (int i = 0; i < 5; i++)
        {
            var response = await _client.GetAsync("/test");
            Assert.True(response.IsSuccessStatusCode);
        }

        var rateLimitedResponse = await _client.GetAsync("/test");
        Assert.Equal(HttpStatusCode.TooManyRequests, rateLimitedResponse.StatusCode);
    }
}