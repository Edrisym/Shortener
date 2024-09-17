namespace Shortener.Endpoints;

public static class ShortenerEndpoint
{
    public static void MapShortenEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/shorten",
                async ([FromBody] ShortenUrl request,
                    IShortenService shortenService,
                    IValidator<ShortenUrl> validator,
                    ILogger<ShortenUrl> logger,
                    CancellationToken cancellationToken,
                    HttpContext context) =>
                {
                    
                    var remoteIpAddress = context.Connection.RemoteIpAddress;
                    logger.LogWarning($"User with Ip address requested a short url. => {remoteIpAddress}");

                    var results = await validator.ValidateAsync(request, cancellationToken);
                    if (!results.IsValid)
                    {
                        logger.LogWarning($"Shortening Failed. => {results}");
                        return Results.ValidationProblem(results.ToDictionary());
                    }


                    var result = await shortenService.MakeShortUrl(request.LongUrl, cancellationToken);
                    logger.LogInformation($"Shortening the URL was successfully done. => {results}");
                    return Results.Ok(new ShortenUrl(result));
                })
            .WithName("Shorten your URL")
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status429TooManyRequests)
            .Produces(StatusCodes.Status200OK, typeof(object))
            .ProducesValidationProblem()
            .RequireRateLimiting("sliding")
            .WithOpenApi();
    }
}