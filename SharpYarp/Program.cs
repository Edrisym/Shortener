using Figgle;

Console.WriteLine(FiggleFonts.Standard.Render("Sharp Yarp Gateway"));

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddEnvironmentVariables();

Console.WriteLine("gateway is running on {0}", builder.Environment.EnvironmentName);

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});
var app = builder.Build();
app.UseCors("AllowAll");
app.MapReverseProxy();

app.Use(async (context, next) =>
{
    var path = context.Request.Path.Value;

    if (string.IsNullOrWhiteSpace(path))
    {
        await next();
        return;
    }

    if (context.Request.Method == "POST")
    {
        context.Request.Path = "/api/v1/blink/shortener/shorten";
    }
    else if (context.Request.Method == "GET")
    {
        var code = path.Trim('/');
        context.Request.Path = "/api/v1/blink/shortener/redirect";
        context.Request.QueryString = new QueryString($"?code={code}");
    }
    else
    {
        context.Response.StatusCode = 405;
        await context.Response.WriteAsync("Method Not Allowed");
        return;
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