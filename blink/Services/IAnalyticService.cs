namespace blink.Services;

public interface IAnalyticService
{
    Task<List<UrlAnalyticResponse>> GetUrlAnalytics(
        UrlAnalyticRequest request,
        CancellationToken cancellationToken);
}

public class AnalyticService(
    ShortenerDbContext db,
    IRedisCacheService redis) : IAnalyticService
{
    public async Task<List<UrlAnalyticResponse>> GetUrlAnalytics(UrlAnalyticRequest request,
        CancellationToken cancellationToken)
    {
        var urls = await db.Urls
            .AsNoTracking()
            .Where(c => c.CreatedBy == request.CreatedBy)
            .ToListAsync(cancellationToken);

        var codes = urls
            .Select(x => x.ShortCode);

        var count = await redis
            .GetVisitCounter(codes);

        return urls.Select(
            x => new UrlAnalyticResponse
            {
                OriginalUrl = x.LongUrl,
                ShortCode = x.ShortCode,
                VisitCount = count
            }).ToList();
    }
}