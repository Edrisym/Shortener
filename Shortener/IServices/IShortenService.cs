namespace Shortener.IServices;

public interface IShortenService
{
    Task<string> ToShortUrl(string longUrl, CancellationToken cancellationToken);
}

public class ShortenService(
    IHashGenerator hashGenerator,
    IOptions<AppSettings> settings,
    ShortenerDbContext dbContext) : IShortenService
{
    private readonly AppSettings _settings = settings.Value;

    public async Task<string> ToShortUrl(string originalUrl, CancellationToken cancellationToken)
    {
        var shortCode = hashGenerator.GenerateShortCode(originalUrl);
        var shortUrl = $"{_settings.BaseUrl}/{shortCode}";

        if (await UrlExists(shortCode, originalUrl, cancellationToken))
            return shortUrl;

        var url = new Url
        {
            LongUrl = originalUrl,
            ShortCode = shortCode
        };

        await dbContext.Urls.AddAsync(url, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        return shortUrl;
    }

    private async Task<bool> UrlExists(string code, string longUrl, CancellationToken cancellationToken)
    {
        return await dbContext.Urls
            .AnyAsync(url => url.ShortCode == code
                             && url.LongUrl == longUrl, cancellationToken);
    }
}