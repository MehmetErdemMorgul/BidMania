using BidMania.AuctionService.Models;
using BidMania.AuctionService.Repositories;

var builder = WebApplication.CreateBuilder(args);

// MongoDB 
builder.Services.Configure<AuctionDatabaseSettings>(
    builder.Configuration.GetSection("AuctionDatabaseSettings"));


builder.Services.AddSingleton<AuctionRepository>();

builder.Services.AddControllers();

var app = builder.Build();


app.Use(async (context, next) =>
{
    if (!context.Request.Headers.TryGetValue("X-Internal-Key", out var extractedKey) ||
        extractedKey != "Kocaeli41_Secret")
    {
        context.Response.StatusCode = 403;
        await context.Response.WriteAsync("Erisim Yasak: Sadece Dispatcher uzerinden gelmelisiniz.");
        return;
    }
    await next();
});

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();

// TESTLER İÇİN ŞART!
public partial class Program { }