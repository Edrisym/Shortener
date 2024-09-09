using MongoDB.Driver;

namespace Shortener.Extensions;

public static class Extensions
{
    public static void AddFluentApiValidation(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddValidatorsFromAssemblyContaining<ShortenUrlValidator>();
        serviceCollection.AddFluentValidationAutoValidation();
    }

    public static void AddServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IShortenService, ShortenService>();
    }
}

public static class MongoDbExtension
{
    public static void AddMongoDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        var mongoClient = new MongoClient(configuration.GetConnectionString("MongoDb"));
        var mongoDatabase = mongoClient.GetDatabase(configuration["DatabaseName"]);
        services.AddScoped(provider => mongoDatabase);
    }
}