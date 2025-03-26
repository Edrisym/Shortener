using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Shortener.Common.Models;
using Shortener.Persistence;
using Moq;
using Shortener.IServices;

namespace Tests
{
    public class Shorten_Service_Tests
    {
        private readonly ShortenService _shortenService;
        private readonly ShortenerDbContext _dbContext;
        private readonly IOptions<AppSettings> _mockOptions;

        public Shorten_Service_Tests()
        {
            var options = new DbContextOptionsBuilder<ShortenerDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _dbContext = new ShortenerDbContext(options);
            _mockOptions = Options.Create(new AppSettings { BaseUrl = "https://short.url/", HashParts = 7 });
            _shortenService = new ShortenService(_mockOptions, _dbContext);
        }

        [Fact]
        public async Task MakeShortUrl_ShouldReturnShortUrl_WhenValidOriginalUrl()
        {
            var originalUrl = "https://example.com";
            var expectedShortCode = _shortenService.GenerateShortCode(originalUrl);

            var result = await _shortenService.ToShortUrl(originalUrl, CancellationToken.None);

            Assert.Equal($"{_mockOptions.Value.BaseUrl}{expectedShortCode}", result);
        }

        [Fact]
        public void GenerateHashing_ShouldReturnValidHash()
        {
            var longUrl = "https://example.com";

            var hash = _shortenService.GenerateShortCode(longUrl);

            Assert.NotEmpty(hash);
        }


        [Fact]
        public async Task MakeShortUrl_ShouldThrowException_WhenDbSaveFails()
        {
            var options = new DbContextOptionsBuilder<ShortenerDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            var mockDbContext = new Mock<ShortenerDbContext>(options);
            mockDbContext.Setup(db => db.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Database error"));

            var serviceWithMockDbContext = new ShortenService(_mockOptions, mockDbContext.Object);

            var originalUrl = "https://example.com";

            await Assert.ThrowsAsync<NullReferenceException>(() =>
                serviceWithMockDbContext.ToShortUrl(originalUrl, CancellationToken.None));
        }
    }
}