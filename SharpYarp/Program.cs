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
            context.Request.Path = "/api/v1/shortener/shorten";
            context.Request.QueryString = new QueryString($"?longUrl={longUrl}");
        }
        else if (context.Request.Method == "GET")
        {
            var code = path.Trim('/');
            context.Request.Path = "/api/v1/shortener/redirect";
            context.Request.QueryString = new QueryString($"?code={code}");
        }
        else
        {
            context.Response.StatusCode = 405;
            await context.Response.WriteAsync("Method Not Allowed");
            return;
        }
    }

    var userAgent = context.Request.Headers["User-Agent"].ToString() ?? "Unknown";
    var referer = context.Request.Headers["Referer"].ToString() ?? "Unknown";
    var userId = context.Request.Headers["UserId"].ToString() ?? "Unknown";
    var ipAddress = context.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

    context.Request.Headers["X-Forwarded-Agent"] = userAgent;
    context.Request.Headers["X-Forwarded-Referer"] = referer;
    context.Request.Headers["X-Forwarded-UserId"] = userId;
    context.Request.Headers["X-Forwarded-IP"] = ipAddress;

    await next();
});
await app.RunAsync();