using Serilog;
using Serilog.Formatting.Compact;
using Serilog.Sinks.Elasticsearch;

namespace Shortener;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        ConfigureSerilog(builder);
        var settings = ConfigureConfigurations(builder);
        ConfigureDbContext(builder, settings.DatabaseSettings);
        builder.Services.AddControllers();
        builder.Services.AddHttpContextAccessor();

        #region CORS

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", policy =>
            {
                policy.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });
        });

        #endregion

        #region Registering services

        builder.Services.AddScoped<IHashGenerator, HashGenerator>();
        builder.Services.AddScoped<IShortenService, ShortenService>();

        #endregion

        var app = builder.Build();

        app.UseSerilogRequestLogging();
        app.UseCors("AllowAll");
        app.UseRouting();
        app.MapControllers();

        await app.RunAsync();
    }

    static void ConfigureSerilog(WebApplicationBuilder builder)
    {
        Log.Logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .Enrich.WithClientIp()
            .Enrich.WithCorrelationId()
            .Enrich.WithProperty("Application", "URL Shortener")
            .Enrich.WithRequestHeader(GatewayHeaders.UserId)
            .Enrich.WithRequestHeader(GatewayHeaders.Agent)
            .Enrich.WithRequestHeader(GatewayHeaders.Referer)
            .Enrich.WithRequestHeader(GatewayHeaders.Ip)
            .Enrich.WithMachineName()
            .Enrich.WithThreadId()
            .Enrich.WithThreadName()
            .WriteTo.Console(new RenderedCompactJsonFormatter())
            .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
            //TODO: add to elastic and remove file logging after testing
            // .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri("http://localhost:9200"))
            // {
            //     AutoRegisterTemplate = true,
            //     IndexFormat = "shortener-logs-{0:yyyy.MM}"
            // })
            .CreateLogger();

        builder.Host.UseSerilog();
    }

    static AppSettings ConfigureConfigurations(WebApplicationBuilder builder)
    {
        builder.Services.AddOptions();
        builder.Services.Configure<AppSettings>
            (builder.Configuration.GetSection(nameof(AppSettings)));

        return builder.Configuration.GetSection("AppSettings").Get<AppSettings>() ??
               throw new Exception("Settings is not configured properly.");
    }

    static void ConfigureDbContext(WebApplicationBuilder builder, DatabaseSettings settings)
    {
        builder.Services.AddDbContext<ShortenerDbContext>(options =>
        {
            options.UseMongoDB(settings.ConnectionString, settings.DatabaseName);
        });
    }
}

//TODO: move this to another namesace
public abstract class GatewayHeaders
{
    public const string UserId = "X-Forwarded-UserId";
    public const string Agent = "X-Forwarded-Agent";
    public const string Referer = "X-Forwarded-Referer";
    public const string Ip = "X-Forwarded-IP";
    public const string GeoLocation = "X-Forwarded-Geo-Location";
    public const string ForwardedHost = "X-Forwarded-Host";
}