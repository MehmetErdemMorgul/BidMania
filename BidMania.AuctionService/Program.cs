using BidMania.AuctionService.Models;
using BidMania.AuctionService.Repositories;

var builder = WebApplication.CreateBuilder(args);

// 1. Ayarları appsettings.json'dan çekip sisteme tanıtıyoruz
builder.Services.Configure<AuctionDatabaseSettings>(
    builder.Configuration.GetSection("AuctionDatabaseSettings"));

// 2. MongoDB ile konuşacak olan Repository'yi (Depocu) sisteme kaydediyoruz
builder.Services.AddSingleton<AuctionRepository>();

// 3. Standart API servisleri
builder.Services.AddControllers();

var app = builder.Build();

// Pipeline Ayarları
if (app.Environment.IsDevelopment())
{
    // Geliştirme aşamasında buraya Swagger vb. eklenebilir
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();

// TESTLER İÇİN KRİTİK SATIR: Sakın silme kanka!
public partial class Program { }