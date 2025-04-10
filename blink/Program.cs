using blink.Common;
using blink.Common.Models;
using blink.Persistence;
using blink.Services;
using Serilog;
using Serilog.Formatting.Compact;
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
        Log.Logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .Enrich.WithClientIp()
            .Enrich.WithCorrelationId()
            .Enrich.WithProperty("Application", "URL blink")
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

    static IServiceCollection ConfigureCors(this IServiceCollection services)
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

    static IServiceCollection RegisterServices(this IServiceCollection service)
    {
        service.AddScoped<IHashGenerator, HashGenerator>()
            .AddScoped<IShortenService, ShortenService>();
        return service;
    }
}