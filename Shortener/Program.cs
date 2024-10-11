var builder = WebApplication.CreateBuilder(args);

builder.ConfigureAppSettings();
builder.ConfigureDbContext();
builder.Services.AddServices();

builder.Services.UseHttpClientMetrics();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMetricServer();
app.UseHttpMetrics();

app.UseForwardedHeaders();
app.MapShortenEndpoint();
app.UseHttpsRedirection();

app.Run();