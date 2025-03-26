using System.Net.NetworkInformation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Shortener.Common.Models;
using Shortener.Persistence;
using Moq;
using Shortener.IServices;

//TODO
namespace Tests
{
    public class ShortenServiceTests
    {
        private readonly IShortenService _shortenService;
        private readonly ShortenerDbContext _dbContext;
        private readonly IOptions<AppSettings> _mockOptions;
        private readonly IHashGenerator _hashGenerator;

        public ShortenServiceTests()
        {
            var options = new DbContextOptionsBuilder<ShortenerDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _dbContext = new ShortenerDbContext(options);
            _mockOptions = Options.Create(new AppSettings { BaseUrl = "https://short.url/" });
            _shortenService = new ShortenService(_hashGenerator, _mockOptions, _dbContext);
        }

        [Fact]
        public async Task Should_Make_Given_Url_Short_Successfully()
        {
            var originalUrl = "https://example.com";
            var expectedShortCode = _shortenService.ToShortUrl(originalUrl, CancellationToken.None);

            var result = await _shortenService.ToShortUrl(originalUrl, CancellationToken.None);

            Assert.Equal($"{_mockOptions.Value.BaseUrl}{expectedShortCode}", result);
        }
    }
}