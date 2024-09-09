using FluentValidation;
using FluentValidation.AspNetCore;
using Shortener.Endpoints;
using Shortener.Extensions;
using Shortener.Models;
using Shortener.Services;
using Shortener.Validator;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddServices();
builder.Services.AddFluentApiValidation();

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