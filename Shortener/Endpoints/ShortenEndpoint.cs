namespace Shortener.Endpoints;

public static class ShortenerEndpoint
{
    public static void MapShortenEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/shorten",
                async ([FromBody] ShortenUrl request,
                    IShortenService shortenService,
                    IValidator<ShortenUrl> validator,
                    CancellationToken cancellationToken) =>
                {
                    var results = await validator.ValidateAsync(request, cancellationToken);

                    if (!results.IsValid)
                    {
                        return Results.ValidationProblem(results.ToDictionary());
                    }

                    var result = await shortenService.MakeShortUrl(request.LongUrl, cancellationToken);
                    return Results.Ok(new { ShortenedUrl = result });
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