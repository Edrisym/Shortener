namespace Shortener;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var settings = ConfigureConfigurations(builder);
        ConfigureDbContext(builder, settings.DatabaseSettings);
        builder.Services.AddControllers();

        #region CORS

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", policy =>
            {
                policy.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });
        });

        #endregion

        #region Registering services

        builder.Services.AddScoped<IHashGenerator, HashGenerator>();
        builder.Services.AddScoped<IShortenService, ShortenService>();

        #endregion

        var app = builder.Build();

        app.UseCors("AllowAll");
        app.UseRouting();
        app.MapControllers();

        await app.RunAsync();
    }

    static AppSettings ConfigureConfigurations(WebApplicationBuilder builder)
    {
        builder.Services.AddOptions();
        builder.Services.Configure<AppSettings>
            (builder.Configuration.GetSection(nameof(AppSettings)));

        return builder.Configuration.GetSection("AppSettings").Get<AppSettings>() ??
               throw new Exception("Settings is not configured properly.");
    }

    static void ConfigureDbContext(WebApplicationBuilder builder, DatabaseSettings settings)
    {
        builder.Services.AddDbContext<ShortenerDbContext>(options =>
        {
            options.UseMongoDB(settings.ConnectionString, settings.DatabaseName);
        });
    }
}