var builder = WebApplication.CreateBuilder(args);
var settings = ConfigureConfigurations(builder);
builder.ConfigureDbContext(settings);
builder.Services.AddScoped<IShortenService, ShortenService>();

var app = builder.Build();

app.UseRouting();

app.MapPost("/shorten",
    async ([FromQuery] string url,
        IShortenService service,
        CancellationToken cancellationToken) =>
    {
        if (!url.IsValid())
        {
            return Results.BadRequest(
                "The provided longUrl is not valid. Please ensure it is a properly formatted URL.");
        }

        var result = await service.ToShortUrl(url, cancellationToken);
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

        // todo : find alternative 
#pragma warning disable SYSLIB0013
        var url = Uri.EscapeUriString(shortenedUrl.LongUrl);
#pragma warning restore SYSLIB0013

        return Results.Redirect(url);
    });

app.MapGet("",
    async (ShortenerDbContext dbContext,
        CancellationToken cancellationToken) =>
    {
        return await dbContext.Urls.Select(x => new
        {
            x.LongUrl,
            x.ShortCode,
            x.CreatedAt,
            x.ExpiresAt
        }).ToListAsync(cancellationToken);
    });

app.Run();

static AppSettings ConfigureConfigurations(WebApplicationBuilder builder)
{
    builder.Services.AddOptions();
    builder.Services.Configure<AppSettings>
        (builder.Configuration.GetSection(nameof(AppSettings)));

    return builder.Configuration.GetSection("AppSettings").Get<AppSettings>() ??
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