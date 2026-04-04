using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

var builder = WebApplication.CreateBuilder(args);


var mongoClient = new MongoClient("mongodb://localhost:27017");
var database = mongoClient.GetDatabase("ProductDb");
var products = database.GetCollection<Product>("Products");

var app = builder.Build();
app.Use(async (context, next) =>
{
    if (!context.Request.Headers.TryGetValue("X-Internal-Key", out var key) || key != "Kocaeli41_Secret")
    {
        context.Response.StatusCode = 403; 
        return;
    }
    await next();
});


app.MapPost("/api/products", async (Product product) =>
{
    await products.InsertOneAsync(product);
    return Results.Created($"/api/products/{product.Id}", product);
});


app.MapGet("/api/products", async () =>
{
    var allProducts = await products.Find(_ => true).ToListAsync();
    return Results.Ok(allProducts); 
});

app.MapGet("/api/products/{id}", async (string id) =>
{
    var product = await products.Find(x => x.Id == id).FirstOrDefaultAsync();
    return product is not null ? Results.Ok(product) : Results.NotFound();
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

public partial class Program { }