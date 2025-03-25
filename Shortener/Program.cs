var builder = WebApplication.CreateBuilder(args);


var settings = ConfigureConfigurations(builder);
builder.ConfigureDbContext(settings);
builder.Services.AddScoped<IShortenService, ShortenService>();

var app = builder.Build();

app.UseRouting();

app.MapPost("/shorten",
    async ([FromQuery] string longUrl,
        IShortenService service,
        CancellationToken cancellationToken) =>
    {
        if (!longUrl.IsValid())
        {
            return Results.BadRequest(
                "The provided longUrl is not valid. Please ensure it is a properly formatted URL.");
        }

        var result = await service.MakeShortUrl(longUrl, cancellationToken);
        return Results.Json(new { LongUrl = result });
    });

app.MapGet("{code}",
    async ([FromRoute] string code,
        ShortenerDbContext dbContext,
        CancellationToken cancellationToken) =>
    {
        var shortenedUrl = await dbContext.Urls.SingleOrDefaultAsync(s => s.ShortCode == code, cancellationToken);

        if (shortenedUrl is null)
        {
            return Results.NotFound();
        }

        return Results.Redirect(Uri.EscapeDataString(shortenedUrl.LongUrl));
    });

app.Run();

static AppSettings ConfigureConfigurations(WebApplicationBuilder builder)
{
    builder.Services.AddOptions();
    builder.Services.Configure<AppSettings>(builder.Configuration);

    return builder.Configuration.Get<AppSettings>() ??
           throw new Exception("Settings is not configured properly.");
}

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