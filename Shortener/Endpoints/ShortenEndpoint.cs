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
                    IValidator<ShortenUrl> validator) =>
                {
                    var results = await validator.ValidateAsync(request);

                    if (!results.IsValid)
                    {
                        return Results.ValidationProblem(results.ToDictionary());
                    }

                    var result = shortenService.MakeShortenUrl(request.Url);
                    return Results.Ok(new { ShortenedUrl = result });
                })
            .WithName("Shorten your URL")
            .ProducesProblem(StatusCodes.Status409Conflict)
            .Produces(StatusCodes.Status200OK, typeof(object))
            .ProducesValidationProblem();
    }
}