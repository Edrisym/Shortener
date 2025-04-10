namespace blink.WebApplicationExtensions;

public static class WebApplicationExtensions
{
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

public static class UrlValidation
{
    const string UrlPattern = @"^(https?:\/\/)?((localhost(:\d{1,5})?)|([a-zA-Z0-9-]+\.)+[a-zA-Z]{2,})(\/[^\s]*)?$";

    public static bool IsUrlValid(this string? value)
    {
        return !string.IsNullOrEmpty(value) && Regex.IsMatch(value!, UrlPattern);
    }
}