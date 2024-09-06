using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Shortener.Common;
using Shortener.Primitives.ApiResultWrapper;
using static Shortener.Primitives.ApiResultWrapper.Result;

namespace Shortener.Endpoints;

public static class ShortenerEndpoint
{
    public static void ShortenEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1");

        //TODO

        group.MapPost("/shorten", async Task<Results<Ok<Result>,
                BadRequest<Result>>>
            ([FromBody] ShortenUrl request) =>
        {
            return TypedResults.Ok(new Result(true, "", []));
        });
    }
}