using blink.Common;
using blink.Services;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using Serilog.Formatting.Compact;
using Serilog.Sinks.Grafana.Loki;
using StackExchange.Redis;

namespace blink;

public static class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args)
            .ConfigureSerilog();

        var settings = builder.ConfigureConfigurations();

        builder.Services.ConfigureDbContext(settings.DatabaseSettings)
            .ConfigureRedis(settings.DatabaseSettings.Redis)
            .RegisterServices()
            .AddHttpContextAccessor()
            .ConfigureCors()
            .ConfigureOpenTelemetry()
            .AddControllers();

        var app = builder.Build();

        app.UseSerilogRequestLogging();
        app.UseCors("AllowAll");
        app.UseRouting();
        app.MapControllers();

        await app.RunAsync();
    }

    static WebApplicationBuilder ConfigureSerilog(this WebApplicationBuilder builder)
    {
        //TODO clean this 

        var labels = new List<LokiLabel>
        {
            new() { Key = "app", Value = "BlinkUrl" },
            // new() { Key = "env", Value = "production" },
            // new() { Key = "env", Value = "development" }
        };

        Log.Logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .Enrich.WithClientIp()
            .Enrich.WithCorrelationId()
            .Enrich.WithProperty("Service", "BlinkUrl")
            .Enrich.With<OpenTelemetryLogEnricher>()
            .Enrich.WithRequestHeader(GatewayHeaders.UserId)
            .Enrich.WithRequestHeader(GatewayHeaders.Agent)
            .Enrich.WithRequestHeader(GatewayHeaders.Referer)
            .Enrich.WithRequestHeader(GatewayHeaders.Ip)
            .Enrich.WithMachineName()
            .Enrich.WithThreadId()
            .Enrich.WithThreadName()
            .WriteTo.Console(new RenderedCompactJsonFormatter())
            // .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
            .WriteTo.GrafanaLoki("http://localhost:3100", labels)

            //TODO: add to elastic and remove file logging after testing
            // .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri("http://localhost:9200"))
            // {
            //     AutoRegisterTemplate = true,
            //     IndexFormat = "shortener-logs-{0:yyyy.MM}"
            // })
            .CreateLogger();

        builder.Host.UseSerilog();
        return builder;
    }

    static AppSettings ConfigureConfigurations(this WebApplicationBuilder builder)
    {
        builder.Services.AddOptions();
        builder.Services.Configure<AppSettings>
            (builder.Configuration.GetSection(nameof(AppSettings)));

        return builder.Configuration.GetSection("AppSettings").Get<AppSettings>() ??
               throw new Exception("Settings is not configured properly.");
    }

    static IServiceCollection ConfigureDbContext(this IServiceCollection services, DatabaseSettings settings)
    {
        services.AddDbContext<ShortenerDbContext>(options =>
        {
            options.UseMongoDB(settings.ConnectionString, settings.DatabaseName);
        });
        return services;
    }

    private static IServiceCollection ConfigureRedis(this IServiceCollection services, Redis settings)
    {
        var connMultiplexer = ConnectionMultiplexer.Connect(settings.Configuration);
        services.AddSingleton<IConnectionMultiplexer>(connMultiplexer);
        services.AddScoped<IRedisCacheService, RedisCacheService>();

        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = settings.Configuration;
            options.InstanceName = settings.InstanceName;
            options.ConnectionMultiplexerFactory = async () => await Task.FromResult(connMultiplexer);
        });
        return services;
    }

    private static IServiceCollection ConfigureCors(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", policy =>
            {
                policy.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });
        });
        return services;
    }

    private static IServiceCollection RegisterServices(this IServiceCollection service)
    {
        service.AddScoped<IHashGenerator, HashGenerator>()
            .AddScoped<IShortenService, ShortenService>()
            .AddScoped<IAnalyticService, AnalyticService>();
        return service;
    }

    private static IServiceCollection ConfigureOpenTelemetry(this IServiceCollection service)
    {
        service.AddOpenTelemetry()
            .ConfigureResource(resource => resource
                .AddService("BlinkUrl")
                .AddAttributes(new[]
                {
                    new KeyValuePair<string, object>("environment", "production"),
                    new KeyValuePair<string, object>("environment", "development"),
                }))
            .WithTracing(tracing => tracing
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                // .AddSqlClientInstrumentation()
                .AddSource("BlinkUrl")
                // .AddConsoleExporter())
                .AddOtlpExporter())
            .WithMetrics(metrics => metrics
                .AddRuntimeInstrumentation()
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                // .AddConsoleExporter())
                .AddOtlpExporter());

        return service;
    }
}