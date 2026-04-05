using BidMania.AuctionService.Models;
using BidMania.AuctionService.Repositories;
using Prometheus;

var builder = WebApplication.CreateBuilder(args);

var isDocker = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true";
var mongoHost = isDocker ? "bidmania_mongo" : "localhost";
builder.Configuration["AuctionDatabaseSettings:ConnectionString"] = $"mongodb://{mongoHost}:27017";

builder.Services.Configure<AuctionDatabaseSettings>(
    builder.Configuration.GetSection("AuctionDatabaseSettings"));

builder.Services.AddSingleton<AuctionRepository>();
builder.Services.AddControllers();

var app = builder.Build();

app.UseHttpMetrics();

app.Use(async (context, next) =>
{
    if (context.Request.Path.StartsWithSegments("/metrics"))
    {
        await next();
        return;
    }

    var remoteIp = context.Connection.RemoteIpAddress?.ToString();
    if (remoteIp == "127.0.0.1" || remoteIp == "::1")
    {
        await next();
        return;
    }

    if (!context.Request.Headers.TryGetValue("X-Internal-Key", out var extractedKey) ||
        extractedKey != "Kocaeli41_Secret")
    {
        context.Response.StatusCode = 403;
        await context.Response.WriteAsync("Erisim Yasak: Sadece Dispatcher uzerinden gelmelisiniz.");
        return;
    }
    await next();
});

app.UseAuthorization();
app.MapControllers();
app.MapMetrics();

app.Run();

public partial class Program { }