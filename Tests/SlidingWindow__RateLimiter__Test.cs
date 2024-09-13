using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Http.Json;
using Shortener;
using Xunit;

public class SlidingWindow__RateLimiter__Test : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public SlidingWindow__RateLimiter__Test(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Shortener_RateLimiter_AllowsRequestsWithinLimit()
    {
        // Arrange
        var client = _factory.CreateClient();

        var requestBody = new { LongUrl = "https://example.com" };

        var tasks = new List<Task<HttpResponseMessage>>();
        for (int i = 0; i < 10; i++)
        {
            tasks.Add(client.PostAsJsonAsync("/shorten", requestBody));
        }

        var responses = await Task.WhenAll(tasks);

        Assert.All(responses, r => Assert.Equal(HttpStatusCode.OK, r.StatusCode));
    }

    [Fact]
    public async Task Shortener_RateLimiter_ThrottlesRequestsOverLimit()
    {
        var client = _factory.CreateClient();
        var requestBody = new { LongUrl = "https://example.com" };

        var tasks = new List<Task<HttpResponseMessage>>();
        for (int i = 0; i < 12; i++)
        {
            tasks.Add(client.PostAsJsonAsync("/shorten", requestBody));
        }

        var responses = await Task.WhenAll(tasks);

        int successCount = responses.Count(r => r.StatusCode == HttpStatusCode.OK);
        int tooManyRequestsCount = responses.Count(r => r.StatusCode == HttpStatusCode.TooManyRequests);

        Assert.Equal(10, successCount);
        Assert.Equal(2, tooManyRequestsCount);
    }

    [Fact]
    public async Task Shortener_RateLimiter_AllowsRequestsAfterWindowSlides()
    {
        var client = _factory.CreateClient();
        var requestBody = new { LongUrl = "https://example.com" };

        var tasks = new List<Task<HttpResponseMessage>>();
        for (int i = 0; i < 10; i++)
        {
            tasks.Add(client.PostAsJsonAsync("/shorten", requestBody));
        }

        var firstResponses = await Task.WhenAll(tasks);

        await Task.Delay(15000);
        var secondResponses = new List<Task<HttpResponseMessage>>();
        for (int i = 0; i < 2; i++)
        {
            secondResponses.Add(client.PostAsJsonAsync("/shorten", requestBody));
        }

        var finalResponses = await Task.WhenAll(secondResponses);

        Assert.All(firstResponses, r => Assert.Equal(HttpStatusCode.OK, r.StatusCode));

        Assert.All(finalResponses, r => Assert.Equal(HttpStatusCode.OK, r.StatusCode));
    }
}