using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using Shortener;

public class SlidingWindowRateLimiterTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public SlidingWindowRateLimiterTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task SlidingWindowRateLimiter_AllowsRequestsWithinLimit()
    {
        var client = _factory.CreateClient();
        var tasks = new List<Task<HttpResponseMessage>>();
        for (int i = 0; i < 10; i++)
        {
            tasks.Add(client.GetAsync("/test"));
        }

        var responses = await Task.WhenAll(tasks);
        Assert.All(responses, r => Assert.Equal(HttpStatusCode.OK, r.StatusCode));
    }

    [Fact]
    public async Task SlidingWindowRateLimiter_ThrottlesRequestsOverLimit()
    {
        var client = _factory.CreateClient();

        var tasks = new List<Task<HttpResponseMessage>>();
        for (int i = 0; i < 12; i++)
        {
            tasks.Add(client.GetAsync("/test"));
        }

        var responses = await Task.WhenAll(tasks);
        int successCount = responses.Count(r => r.StatusCode == HttpStatusCode.OK);
        int tooManyRequestsCount = responses.Count(r => r.StatusCode == HttpStatusCode.TooManyRequests);

        Assert.Equal(10, successCount);
        Assert.Equal(2, tooManyRequestsCount);
    }

    [Fact]
    public async Task SlidingWindowRateLimiter_AllowsNewRequestsAfterWindowSlides()
    {
        var client = _factory.CreateClient();

        var tasks = new List<Task<HttpResponseMessage>>();
        for (int i = 0; i < 10; i++)
        {
            tasks.Add(client.GetAsync("/test"));
        }

        var firstResponses = await Task.WhenAll(tasks);
        var secondResponses = new List<Task<HttpResponseMessage>>();
        for (int i = 0; i < 2; i++)
        {
            secondResponses.Add(client.GetAsync("/test"));
        }

        var finalResponses = await Task.WhenAll(secondResponses);
        Assert.All(firstResponses, r => Assert.Equal(HttpStatusCode.OK, r.StatusCode));
        Assert.All(finalResponses, r => Assert.Equal(HttpStatusCode.OK, r.StatusCode));
    }
}