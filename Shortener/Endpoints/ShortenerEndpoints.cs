namespace Shortener.Endpoints;

public static class ShortenerEndpoints
{
    public static void MapShortenerEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/shorten", ShortenUrl);
        app.MapGet("/redirect", RedirectToUrl);
        app.MapGet("", GetUrls);
    }

    private static async Task<List<UrlResponse>> GetUrls(
        [FromServices] ShortenerDbContext dbContext,
        CancellationToken cancellationToken)
    {
        return await dbContext.Urls.Select(x =>
                new UrlResponse(x.LongUrl, x.ShortCode, x.CreatedAt, x.ExpiresAt))
            .ToListAsync(cancellationToken);
    }

    private static async Task<IResult> RedirectToUrl(
        [FromQuery(Name = "code")] string code,
        ShortenerDbContext dbContext,
        CancellationToken cancellationToken)
    {
        var request = new RedirectRequest
        {
            Code = code
        };
        var shortenedUrl = await dbContext.Urls
            .SingleOrDefaultAsync(s => s.ShortCode == request.Code, cancellationToken);

        if (shortenedUrl is null)
            return Results.NotFound("No Url is found");

        if (DateTime.UtcNow > shortenedUrl.ExpiresAt)
            return Results.Problem(
                "The Code is Expired. Try shortening the URL again.",
                statusCode: StatusCodes.Status422UnprocessableEntity);

#pragma warning disable SYSLIB0013
        // todo : find an alternative 
        var url = Uri.EscapeUriString(shortenedUrl.LongUrl);
#pragma warning restore SYSLIB0013

        return Results.Redirect(url);
    }

    private static async Task<IResult> ShortenUrl(
        [FromQuery(Name = "longUrl")] string longUrl,
        [FromServices] IShortenService service,
        CancellationToken cancellationToken)
    {
        var request = new ShortenUrlRequest
        {
            LongUrl = longUrl
        };

        var result = await service.ToShortUrl(request, cancellationToken);
        return Results.Json(new { LongUrl = result });
    }
}