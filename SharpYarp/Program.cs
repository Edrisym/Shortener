using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Yarp.ReverseProxy;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();

app.MapReverseProxy();

app.Use(async (context, next) =>
{
    var path = context.Request.Path.Value;
    if (path is not null)
    {
        if (context.Request.Method == "POST")
        {
            var longUrl = path.Trim('/');
            context.Request.Path = "/api/v1/urls/shorten";
            context.Request.QueryString = new QueryString($"?longUrl={longUrl}");
        }
        else if (context.Request.Method == "GET")
        {
            var code = path.Trim('/');
            context.Request.Path = "/api/v1/urls/redirect";
            context.Request.QueryString = new QueryString($"?code={code}");
        }
        else
        {
            context.Response.StatusCode = 405;
            await context.Response.WriteAsync("Method Not Allowed");
            return;
        }
    }

    await next();
});
await app.RunAsync();