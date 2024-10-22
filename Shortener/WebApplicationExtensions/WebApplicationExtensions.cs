using Shortener.Common.Models;
using Microsoft.AspNetCore.RateLimiting;

namespace Shortener.WebApplicationExtensions;

public static class WebApplicationExtensions
{
    public static void ConfigureAppSettings(this WebApplicationBuilder builder)
    {
        builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));
    }

    public static void AddServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IShortenService, ShortenService>();
    }

    public static void ConfigureDbContext(this WebApplicationBuilder builder)
    {
        var settings = builder.Configuration.GetSection("AppSettings").Get<AppSettings>();
        builder.Services.AddDbContext<ShortenerDbContext>(options =>
        {
            if (settings is null)
                throw new ArgumentNullException(nameof(settings));
            options.UseMongoDB(settings.ConnectionString, settings.DatabaseName);
        });
    }

    public static void AddRateLimiting(this WebApplicationBuilder builder)
    {
        var myOptions = builder.Configuration.GetSection("MyRateLimitOptions").Get<MyRateLimitOptions>();

        var slidingPolicy = "sliding";

        builder.Services.AddRateLimiter(rateLimiterOptions =>
        {
            rateLimiterOptions.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            rateLimiterOptions.AddSlidingWindowLimiter(policyName: slidingPolicy, options =>
            {
                options.PermitLimit = myOptions!.PermitLimit;
                options.Window = TimeSpan.FromSeconds(myOptions.Window);
                options.SegmentsPerWindow = myOptions.SegmentsPerWindow;
                // options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                // options.QueueLimit = myOptions.QueueLimit;
            });
        });
    }
}