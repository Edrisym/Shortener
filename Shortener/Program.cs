using Shortener.Endpoints;

var builder = WebApplication.CreateBuilder(args);
var settings = ConfigureConfigurations(builder);
ConfigureDbContext(builder, settings);

builder.Services.AddScoped<IHashGenerator, HashGenerator>();
builder.Services.AddScoped<IShortenService, ShortenService>();

var app = builder.Build();

app.UseRouting();

app.MapGroup("/api/v1/url")
    .WithTags("shortener APIs")
    .MapShortenerEndpoints();

app.Run();

static AppSettings ConfigureConfigurations(WebApplicationBuilder builder)
{
    builder.Services.AddOptions();
    builder.Services.Configure<AppSettings>
        (builder.Configuration.GetSection(nameof(AppSettings)));

    return builder.Configuration.GetSection("AppSettings").Get<AppSettings>() ??
           throw new Exception("Settings is not configured properly.");
}

static void ConfigureDbContext(WebApplicationBuilder builder, AppSettings settings)
{
    builder.Services.AddDbContext<ShortenerDbContext>(options =>
    {
        options.UseMongoDB(settings.ConnectionString, settings.DatabaseName);
    });
}

public partial class Program
{
}