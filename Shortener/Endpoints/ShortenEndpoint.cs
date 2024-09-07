using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Shortener.Common;
using Shortener.Services;

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

                    var result = await shortenService.MakeShortenUrl(request.LongUrl, cancellationToken);
                    return Results.Ok(new { ShortenedUrl = result });
                })
            .WithName("Shorten your URL")
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status200OK, typeof(object))
            .ProducesValidationProblem()
            .WithOpenApi();
    }
}