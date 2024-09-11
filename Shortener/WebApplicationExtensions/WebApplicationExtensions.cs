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

    public static void ConfigureAppSetting(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        serviceCollection.Configure<AppSettings>(configuration.GetSection("AppSettings"));
    }

    public static void AddServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IShortenService, ShortenService>();
    }


    public static void ConfigureDbContext(this WebApplicationBuilder builder)
    {
        var settings = builder.Configuration.Get<AppSettings>();
        builder.Services.AddDbContext<ShortenerDbContext>(options =>
        {
            if (settings is null)
                throw new ArgumentNullException(nameof(settings));

            options.UseMongoDB(settings.ConnectionString, settings.DatabaseName);
        });
    }
}