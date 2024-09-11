using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using Shortener.Common.Models;
using Shortener.Persistence;

namespace Shortener.WebApplicationExtensions;

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

            var client = new MongoClient(settings.ConnectionString);
            var db = client.GetDatabase("edrisym_shortener");
            var shortUrls = db.GetCollection<ShortUrl>("shortUrl");

            var field = new StringFieldDefinition<ShortUrl>("ShortCode");
            var indexDefinition = new IndexKeysDefinitionBuilder<ShortUrl>()
                .Ascending(field);

            var opt = new CreateIndexOptions { Unique = true };
            shortUrls.Indexes.CreateOne(indexDefinition, opt);

            options.UseMongoDB(client, settings.DatabaseName);
        });
    }
}