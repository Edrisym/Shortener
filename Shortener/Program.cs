using Shortener.Endpoints;
using Shortener.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.ConfigureAppSettings();
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


app.Run();