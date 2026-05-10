using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using NasaApp.Services;

var builder = WebApplication.CreateBuilder(args);

// Dependency injection
builder.Services.AddSingleton<NasaApiService>();

// Telegram-Bot in BG
builder.Services.AddHostedService<TelegramBotService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "NASA API", Version = "v1" });});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "NASA API v1");
    c.RoutePrefix = string.Empty;
});

app.MapControllers();

app.Run();