using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Moq;
using blink.Common.Models;
using blink.Controllers.User.DTOs.Requests;
using blink.Persistence;
using blink.Services;

namespace Tests;

public class ShortenServiceTests : IClassFixture<BaseFixture>
{
    private readonly ShortenService _service;
    private readonly Mock<IHashGenerator> _hashGeneratorMock = new();
    private readonly Mock<IRedisCacheService> _redisMock = new();
    private readonly ShortenerDbContext _dbContext;

    public ShortenServiceTests(BaseFixture fixture)
    {
        _dbContext = fixture.CreateDatabase();

        var settingsMock = new Mock<IOptions<AppSettings>>();
        settingsMock.Setup(s => s.Value).Returns(new AppSettings
        {
            UrlSettings = new UrlSettings { BaseUrls = new BaseUrls { Gateway = "http://localhost:5255" } }
        });

        _service = new ShortenService(_hashGeneratorMock.Object, settingsMock.Object, _dbContext, _redisMock.Object);
    }

    [Fact]
    public async Task ToShortUrl_Should_Return_ShortUrl()
    {
        // Arrange
        var longUrl = "https://example.com";
        var request = new ShortenUrlRequest { LongUrl = longUrl };
        _hashGeneratorMock.Setup(h => h.GenerateShortCode(longUrl)).Returns("abc123");

        // Act
        var result = await _service.ToShortUrl(request, CancellationToken.None);

        // Assert
        Assert.Equal("http://localhost:5255/abc123", result);
        Assert.Contains(await _dbContext.Urls.ToListAsync(), u => u.LongUrl == longUrl);
    }

    [Fact]
    [Obsolete("Obsolete")]
    public async Task RedirectToUrl_Should_Return_LongUrl_When_Cached()
    {
        // Arrange
        var request = new RedirectRequest { Code = "abc123" };
        var url = new Url(longUrl: "https://example.com", shortCode: "abc123");
        _redisMock.Setup(r => r.GetCacheValueAsync<Url>("abc123")).ReturnsAsync(url);

        // Act
        var result = await _service.RedirectToUrl(request, CancellationToken.None);

        // Assert
        Assert.Equal(Uri.EscapeUriString("https://example.com"), result);
    }

    [Fact]
    public async Task GetUrls_Should_Return_List_Of_Urls()
    {
        // Arrange
        var sampleUrl = "https://Anotherexample.com";
        var sampleShortCode = "abc123";
        
        _dbContext.Urls.Add(new Url(longUrl: sampleUrl, shortCode: sampleShortCode));
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _service.GetUrls(CancellationToken.None);

        // Assert
        Assert.Contains(result, u =>
            u.LongUrl == sampleUrl
            && u.ShortCode == sampleShortCode);
    }
}