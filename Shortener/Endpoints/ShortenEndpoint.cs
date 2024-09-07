using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Shortener.Common;
using Shortener.Primitives.ApiResultWrapper;
using Shortener.Primitives.Response;
using Shortener.Services;

namespace Shortener.Endpoints;

public static class ShortenerEndpoint
{
    public static void MapShortenEndpoint(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1");

        //TODO

        group.MapPost("/shorten",
                async Task<Results<Ok<Result>, BadRequest<Result>>>
                    ([FromBody] ShortenUrl request, IShortenService shortenService) =>
                {
                    var result = shortenService.ShortenUrl(request.Url);

                    //TODO
                    if (string.IsNullOrWhiteSpace(result))
                    {
                        return Result.Failure();
                    }

                    return TypedResults.Ok(new Result(true, "", []));
                })
            .AddEndpointFilter<ShortenerEndpointFilter>()
            .WithName("Shorten you URL")
            .ProducesProblem(StatusCodes.Status409Conflict)
            .ProducesValidationProblem();
    }
}

public class ShortenerEndpointFilter : IEndpointFilter
{
    public ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        throw new NotImplementedException();
    }
}