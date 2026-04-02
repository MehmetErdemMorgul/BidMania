using BidMania.AuctionService.Models;
using BidMania.AuctionService.Repositories;

var builder = WebApplication.CreateBuilder(args);

// 1. MongoDB Ayarları
builder.Services.Configure<AuctionDatabaseSettings>(
    builder.Configuration.GetSection("AuctionDatabaseSettings"));

// 2. Repository Kaydı (Mutfaktaki malzemeler)
builder.Services.AddSingleton<AuctionRepository>();

// 3. API Servisleri
builder.Services.AddControllers();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();

// TESTLER İÇİN ŞART!
public partial class Program { }