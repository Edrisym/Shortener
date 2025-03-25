using Shortener.Common.Models;
using Microsoft.AspNetCore.RateLimiting;

namespace Shortener.WebApplicationExtensions;

public static class WebApplicationExtensions
{


    public static void ConfigureDbContext(this WebApplicationBuilder builder , AppSettings settings)
    {
        builder.Services.AddDbContext<ShortenerDbContext>(options =>
        {
            options.UseMongoDB(settings.ConnectionString, settings.DatabaseName);
        });
    }

    // public static void AddRateLimiting(this WebApplicationBuilder builder)
    // {
    //     var myOptions = builder.Configuration.GetSection("MyRateLimitOptions").Get<MyRateLimitOptions>();
    //
    //     var slidingPolicy = "sliding";
    //
    //     builder.Services.AddRateLimiter(rateLimiterOptions =>
    //     {
    //         rateLimiterOptions.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    //         rateLimiterOptions.AddSlidingWindowLimiter(policyName: slidingPolicy, options =>
    //         {
    //             options.PermitLimit = myOptions!.PermitLimit;
    //             options.Window = TimeSpan.FromSeconds(myOptions.Window);
    //             options.SegmentsPerWindow = myOptions.SegmentsPerWindow;
    //             // options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
    //             // options.QueueLimit = myOptions.QueueLimit;
    //         });
    //     });
    // }
}