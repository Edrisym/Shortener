using Shortener.Endpoints;
using Shortener.Extensions;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddServices();
builder.Services.AddFluentApiValidation();
builder.Services.AddMongoDatabase(builder.Configuration);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<StaticDataOption>(
    builder.Configuration.GetSection("StaticDataOption"));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapShortenEndpoint();

app.UseHttpsRedirection();


app.Run();