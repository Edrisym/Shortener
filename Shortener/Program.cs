using SharpCompress.Compressors.Xz;
using Shortener.Endpoints;
using Shortener.WebApplicationExtensions;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.ConfigureAppSettings();
        builder.AddRateLimiting();
        builder.ConfigureDbContext();
        builder.Services.AddServices();
        builder.Services.AddFluentApiValidation();

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();


        var app = builder.Build();
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.MapShortenEndpoint();
        app.UseHttpsRedirection();

        app.MapGet("/test", () => Results.Ok("Rate Limiter Test"));


        app.Run();
    }
}