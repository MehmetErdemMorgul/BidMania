using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

var builder = WebApplication.CreateBuilder(args);

// 1. Baðýmsýz NoSQL: ProductDb (AuctionDb'den tamamen ayrý ?)
var mongoClient = new MongoClient("mongodb://localhost:27017");
var productDb = mongoClient.GetDatabase("ProductDb");
var productsCollection = productDb.GetCollection<Product>("Products");

builder.Services.AddControllers();
var app = builder.Build();

// 2. NETWORK ISOLATION (Að Ýzolasyonu): Sadece Dispatcher'ý içeri alýr ?
app.Use(async (context, next) =>
{
    // Eðer kafasýnda (Header) bizim gizli þifremiz yoksa 403 verip kovuyoruz
    if (!context.Request.Headers.TryGetValue("X-Internal-Key", out var extractedKey) ||
        extractedKey != "Kocaeli41_Secret")
    {
        context.Response.StatusCode = 403; // Forbidden
        await context.Response.WriteAsync("Dýþarýdan eriþim yasak kanka! Dispatcher üzerinden gel.");
        return;
    }
    await next();
});

app.MapGet("/api/products", async () => await productsCollection.Find(_ => true).ToListAsync());

app.MapPost("/api/products", async (Product product) =>
{
    await productsCollection.InsertOneAsync(product);
    return Results.Created($"/api/products/{product.Id}", product);
});

app.Run();

public class Product
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }
    public string Name { get; set; } = null!;
    public decimal Price { get; set; }
}