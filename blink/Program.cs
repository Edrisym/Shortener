using blink.Common;
using blink.Services;
using Figgle;
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
        Console.WriteLine(FiggleFonts.Standard.Render("Blink Url Shortener"));
        var builder = WebApplication.CreateBuilder(args)
            .ConfigureSerilog();

        builder.Configuration
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
            .AddEnvironmentVariables();

        Console.WriteLine("app is running on {0}", builder.Environment.EnvironmentName);

        builder.ConfigureOpenTelemetry();

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
        app.UseOpenTelemetryPrometheusScrapingEndpoint();

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
            // .WriteTo.GrafanaLoki("http://localhost:3100", labels)

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
            Console.WriteLine("----{0}", settings.InstanceName);
            Console.WriteLine("----{0}", settings.Configuration);
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

    private static void ConfigureOpenTelemetry(this WebApplicationBuilder builder)
    {
        var openTelemetry = builder.Services.AddOpenTelemetry()
            .ConfigureResource(resource => resource
                .AddService(serviceName: builder.Environment.ApplicationName)
                .AddAttributes(new Dictionary<string, object>
                {
                    ["deployment.environment"] = builder.Environment.EnvironmentName.ToLower(),
                    ["service.instance.id"] = Environment.MachineName,
                    ["host.name"] = Environment.MachineName,
                    ["telemetry.sdk.language"] = "dotnet"
                }));

        openTelemetry.WithMetrics(metrics =>
        {
            metrics
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddRuntimeInstrumentation()
                .AddProcessInstrumentation()
                .AddMeter(builder.Environment.ApplicationName);

            // if (builder.Environment.IsDevelopment())
            // metrics.AddConsoleExporter();
            metrics.AddPrometheusExporter(options => { options.ScrapeEndpointPath = "/metrics"; });
            // else
            //     metrics.AddOtlpExporter();
        });

        openTelemetry.WithTracing(tracing =>
        {
            tracing
                .AddAspNetCoreInstrumentation(options =>
                {
                    options.RecordException = true;
                    options.Filter = _ => true;
                })
                .AddHttpClientInstrumentation()
                .AddRedisInstrumentation();

            // if (builder.Environment.IsDevelopment())
            // tracing.AddConsoleExporter();
            // else
            // tracing.AddOtlpExporter();

            // if (!builder.Environment.IsDevelopment())
            //     openTelemetry.UseOtlpExporter(OpenTelemetry.Exporter.OtlpExportProtocol.HttpProtobuf,
            //         new Uri(settings.OpenTelemetrySettings.ExporterUrl));
        });
    }
}