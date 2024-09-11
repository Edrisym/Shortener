using System.Security.Authentication;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using Shortener.Common.Models;
using Shortener.Persistence;

namespace Shortener.Extensions;

public static class WebApplicationExtensions
{
    public static void AddFluentApiValidation(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddValidatorsFromAssemblyContaining<ShortenUrlValidator>();
        serviceCollection.AddFluentValidationAutoValidation();
    }

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
}