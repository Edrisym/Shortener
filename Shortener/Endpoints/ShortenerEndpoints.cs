using Microsoft.EntityFrameworkCore;
using Shortener.Persistence;

namespace Shortener.Endpoints;

public static class ShortenerEndpoint
{
    public static void MapShortenEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/shorten",
                async ([FromBody] ShortenUrl request,
                    IShortenService shortenService,
                    ILogger<ShortenUrl> logger,
                    CancellationToken cancellationToken,
                    HttpContext context) =>
                {
                    var remoteIpAddress = context.Connection.RemoteIpAddress;
                    logger.LogWarning($"User with Ip address requested a short url. => {remoteIpAddress}");

                    if (!request.LongUrl.IsValid())
                    {
                        return Results.ValidationProblem(new Dictionary<string, string[]>
                            {
                                { "LongUrl", ["Please provide a valid URL.", "Error message 2"] },
                            }
                        );
                    }

                    var result = await shortenService.MakeShortUrl(request.LongUrl, cancellationToken);
                    logger.LogInformation($"Shortening the URL was successfully done. => {request.LongUrl}");
                    return Results.Ok(new ShortenUrl { LongUrl = result });
                })
            .RequireRateLimiting("sliding")
            .WithOpenApi();


        app.MapGet("{code}", async (
                string code,
                ShortenerDbContext dbContext,
                ILogger<ShortenUrl> logger,
                CancellationToken cancellationToken,
                HttpContext context) =>
            {
                var remoteIpAddress = context.Connection.RemoteIpAddress;
                logger.LogWarning($"User with Ip address requested a short url. => {remoteIpAddress}");

                var shortenedUrl = await dbContext.ShortUrl
                    .SingleOrDefaultAsync(s => s.ShortCode == code, cancellationToken);

                if (shortenedUrl is null)
                {
                    return Results.NotFound();
                }

                var encodedUrl = Uri.EscapeUriString(shortenedUrl.OriginalUrl);
                return Results.Redirect(encodedUrl);
            }).RequireRateLimiting("sliding")
            .WithOpenApi();
        ;
    }
}