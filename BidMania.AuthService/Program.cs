using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

var builder = WebApplication.CreateBuilder(args);

var mongoClient = new MongoClient("mongodb://localhost:27017");
var productDb = mongoClient.GetDatabase("ProductDb");
var productsCollection = productDb.GetCollection<Product>("Products");

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


app.MapGet("/api/products", async () => await productsCollection.Find(_ => true).ToListAsync());
app.MapPost("/api/products", async (Product product) =>
{
    await productsCollection.InsertOneAsync(product);
    return Results.Created($"/api/products/{product.Id}", product);
});

app.MapControllers(); 
app.Run();


public class Product
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }
    public string Name { get; set; } = null!;
    public decimal Price { get; set; }
}


public partial class Program { }