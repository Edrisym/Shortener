namespace blink.Services;

public interface IAnalyticService
{
    Task<List<UrlAnalyticResponse>> GetUrlAnalytics(
        UrlAnalyticRequest request,
        CancellationToken cancellationToken);
}

public class AnalyticService(ShortenerDbContext db) : IAnalyticService
{
    public async Task<List<UrlAnalyticResponse>> GetUrlAnalytics(UrlAnalyticRequest request,
        CancellationToken cancellationToken)
    {
        return await db.Urls
            .Where(c => c.CreatedBy == request.CreatedBy)
            .Select(x => new UrlAnalyticResponse
            {
                OriginalUrl = x.LongUrl,
                ShortCode = x.ShortCode,
                // ViewCount = x.Viewers.Count
            }).ToListAsync(cancellationToken);
    }
}