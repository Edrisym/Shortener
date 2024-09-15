using Xunit;
using Moq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.TestHost;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shortener.Common;
using Shortener.IServices;

public class ShortenerEndpoint_Tests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public ShortenerEndpoint_Tests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task ShortenEndpoint_Returns_Validation_Error_When_Invalid()
    {
        var mockValidator = new Mock<IValidator<ShortenUrl>>();
        var mockShortenService = new Mock<IShortenService>();

        var invalidRequest = new ShortenUrl(String.Empty);
        var validationResult = new ValidationResult(new[] { new ValidationFailure("LongUrl", "URL cannot be empty") });
        mockValidator
            .Setup(v => v.ValidateAsync(invalidRequest, It.IsAny<CancellationToken>()))
            .ReturnsAsync(validationResult);

        var client = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                services.AddScoped(_ => mockValidator.Object);
                services.AddScoped(_ => mockShortenService.Object);
            });
        }).CreateClient();

        var response = await client.PostAsJsonAsync("/shorten", invalidRequest);

        Assert.Equal(StatusCodes.Status400BadRequest, (int)response.StatusCode);
        var problemDetails = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        Assert.Contains("LongUrl", problemDetails.Errors.Keys);
    }

    [Fact]
    public async Task ShortenEndpoint_Returns_Shortened_Url_On_Success()
    {
        var mockValidator = new Mock<IValidator<ShortenUrl>>();
        var mockShortenService = new Mock<IShortenService>();

        var validRequest = new ShortenUrl("https://example.com");
        var validationResult = new ValidationResult();

        mockValidator
            .Setup(v => v.ValidateAsync(validRequest, It.IsAny<CancellationToken>()))
            .ReturnsAsync(validationResult);

        mockShortenService
            .Setup(s => s.MakeShortUrl(validRequest.LongUrl, It.IsAny<CancellationToken>()))
            .ReturnsAsync("https://short.url/abc123");

        var client = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                services.AddScoped(_ => mockValidator.Object);
                services.AddScoped(_ => mockShortenService.Object);
            });
        }).CreateClient();

        var response = await client.PostAsJsonAsync("/shorten", validRequest);

        Assert.Equal(StatusCodes.Status200OK, (int)response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<ShortenUrl>();

        Assert.Equal("https://short.url/abc123", result.LongUrl);
    }
}