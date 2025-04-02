namespace Shortener.Services;

public interface IShortenService
{
    Task<string> ToShortUrl(ShortenUrlRequest longUrl, CancellationToken cancellationToken);
    Task<string?> RedirectToUrl(RedirectRequest request, CancellationToken cancellationToken);
    Task<List<UrlResponse>> GetUrls(CancellationToken cancellationToken);
    Task<UrlResponse?> GetUrl(string id, CancellationToken cancellationToken);
}

public class ShortenService(
    IHashGenerator hashGenerator,
    IOptions<AppSettings> settings,
    ShortenerDbContext dbContext,
    IRedisCacheService redis) : IShortenService
{
    private readonly UrlSettings _settings = settings.Value.UrlSettings;

    public async Task<string> ToShortUrl(
        ShortenUrlRequest request,
        CancellationToken cancellationToken)
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
        await redis.SetCacheValueAsync(url.ShortCode, url, expiration: TimeSpan.FromSeconds(60));

        return shortUrl;
    }

    [Obsolete("Obsolete")]
    public async Task<string?> RedirectToUrl(
        RedirectRequest request,
        CancellationToken cancellationToken)
    {
        var url = await redis.GetCacheValueAsync<Url>(request.Code) ??
                  await dbContext.Urls.SingleOrDefaultAsync(s =>
                      s.ShortCode == request.Code, cancellationToken);

        if (url is null)
            return default;

        await redis.SetCacheValueAsync(request.Code, url, expiration: TimeSpan.FromSeconds(60));
        await redis.PersistAndIncrementCounterAsync(url.ShortCode);

        // TODO: find an alternative
        return Uri.EscapeUriString(url.LongUrl);

        //TODO
        // if (DateTime.UtcNow > url.ExpiresAt)
        //     throw new Exception("The Code is Expired. Try shortening the URL again.");
    }

    public async Task<List<UrlResponse>> GetUrls(
        CancellationToken cancellationToken)
    {
        return await dbContext.Urls
            .Select(x => new UrlResponse(x.LongUrl, x.ShortCode, x.CreatedAt, x.ExpiresAt))
            .ToListAsync(cancellationToken);
    }

    public async Task<UrlResponse?> GetUrl(
        string id,
        CancellationToken cancellationToken)
    {
        return await dbContext.Urls
            .Where(s => s.Id == id)
            .Select(x => new UrlResponse(x.LongUrl, x.ShortCode, x.CreatedAt, x.ExpiresAt))
            .FirstOrDefaultAsync(cancellationToken);
    }

    private async Task<bool> UrlExists(
        string code,
        string longUrl,
        CancellationToken cancellationToken)
    {
        return await dbContext.Urls
            .AnyAsync(url => url.ShortCode == code
                             && url.LongUrl == longUrl, cancellationToken);
    }
}