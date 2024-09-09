namespace Shortener.Extensions;
public static class DependencyInjections
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