

namespace Shortener.IServices;

public interface IShortenService
{
    Task<string> ToShortUrl(ShortenUrlRequest longUrl, CancellationToken cancellationToken);
}

public class ShortenService(
    IHashGenerator hashGenerator,
    IOptions<AppSettings> settings,
    ShortenerDbContext dbContext) : IShortenService
{
    private readonly UrlSettings _settings = settings.Value.UrlSettings;

    public async Task<string> ToShortUrl(ShortenUrlRequest request, CancellationToken cancellationToken)
    {
        var shortCode = hashGenerator.GenerateShortCode(request.LongUrl);
        var shortUrl = $"{_settings.BaseUrls.Gateway}/{shortCode}";
        if (await UrlExists(shortCode, request.LongUrl, cancellationToken))
            return shortUrl;

        var url = new Url
        {
            LongUrl = request.LongUrl,
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