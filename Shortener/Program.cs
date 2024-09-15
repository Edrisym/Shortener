// Put Program here because tests wouldn't recognise the class  

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

        app.UseRateLimiter();

        app.Run();
    }
}