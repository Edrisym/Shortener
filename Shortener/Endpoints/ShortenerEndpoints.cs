using Microsoft.AspNetCore.Http.HttpResults;

namespace Shortener.Endpoints;

public static class ShortenerEndpoints
{
    public static IEndpointRouteBuilder MapShortenerEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/shorten", ShortenUrl);
        app.MapGet("{code}/redirect", RedirectToUrl);
        app.MapGet("", GetUrls);
        return app;
    }

    private static async Task<List<UrlRecord>> GetUrls
    (ShortenerDbContext dbContext,
        CancellationToken cancellationToken)
    {
        return await dbContext.Urls.Select(x => new UrlRecord(x.LongUrl, x.ShortCode, x.CreatedAt, x.ExpiresAt))
            .ToListAsync(cancellationToken);
    }

    private static async Task<IResult> RedirectToUrl
    ([FromRoute] string code,
        ShortenerDbContext dbContext,
        CancellationToken cancellationToken)
    {
        var shortenedUrl = await dbContext.Urls
            .SingleOrDefaultAsync(s => s.ShortCode == code, cancellationToken);

        if (shortenedUrl is null)
            return Results.NotFound();

        if (DateTime.UtcNow > shortenedUrl.ExpiresAt)
            return Results.Problem(
                "The Code is Expired. Try shortening the URL again.",
                statusCode: StatusCodes.Status422UnprocessableEntity);

        // todo : find alternative 
#pragma warning disable SYSLIB0013
        var url = Uri.EscapeUriString(shortenedUrl.LongUrl);
#pragma warning restore SYSLIB0013

        return Results.Redirect(url);
    }

    private static async Task<IResult> ShortenUrl
    ([FromQuery] string url,
        IShortenService service,
        CancellationToken cancellationToken)
    {
        if (!url.IsUrlValid())
        {
            return Results.BadRequest(
                "The provided longUrl is not valid. Please ensure it is a properly formatted URL.");
        }

        var result = await service.ToShortUrl(url, cancellationToken);
        return Results.Json(new { LongUrl = result });
    }

    private record UrlRecord(
        string LongUrl,
        string ShortCode,
        DateTime CreatedAt,
        DateTime ExpiresAt);
}