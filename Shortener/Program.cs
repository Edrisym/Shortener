var builder = WebApplication.CreateBuilder(args);

builder.ConfigureAppSettings();
builder.ConfigureDbContext();
builder.Services.AddServices();

var app = builder.Build();

app.UseMiddleware<RequestResponseLoggingMiddleware>();
app.UseRouting();


app.MapPost("/shorten",
    async ([FromQuery] string longUrl,
        IShortenService shortenService,
        CancellationToken cancellationToken) =>
    {
        if (!longUrl.IsValid())
        {
            return Results.BadRequest("The provided longUrl is not valid. Please ensure it is a properly formatted URL.");
        }

        var result = await shortenService.MakeShortUrl(longUrl, cancellationToken);
        return Results.Json(new ShortenUrl { LongUrl = result });
    });

app.MapGet("{code}",
    async (string code,
        ShortenerDbContext dbContext,
        CancellationToken cancellationToken) =>
    {
        var shortenedUrl = await dbContext.ShortUrl.SingleOrDefaultAsync(s => s.ShortCode == code, cancellationToken);

        if (shortenedUrl is null)
        {
            return Results.NotFound();
        }

        var encodedUrl = Uri.EscapeDataString(shortenedUrl.OriginalUrl);
        return Results.Redirect(encodedUrl);
    });

app.Run();

public static class UrlValidation
{
    const string UrlPattern = @"^(https?:\/\/)?((localhost(:\d{1,5})?)|([a-zA-Z0-9-]+\.)+[a-zA-Z]{2,})(\/[^\s]*)?$";

    public static bool IsValid(this string? value)
    {
        return !string.IsNullOrEmpty(value) && Regex.IsMatch(value!, UrlPattern);
    }
}

public partial class Program
{
}