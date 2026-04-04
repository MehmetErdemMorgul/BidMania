using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

var builder = WebApplication.CreateBuilder(args);


var mongoClient = new MongoClient("mongodb://localhost:27017");
var database = mongoClient.GetDatabase("ProductDb");
var products = database.GetCollection<ProductItem>("Products");

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


app.MapPost("/api/products", async (ProductItem item) =>
{
    await products.InsertOneAsync(item);
    return Results.Created($"/api/products/{item.Id}", item);
});


app.MapGet("/api/products", async () =>
{
    var all = await products.Find(_ => true).ToListAsync();
    return Results.Ok(all);
});


app.MapGet("/api/products/{id}", async (string id) =>
{
    var item = await products.Find(x => x.Id == id).FirstOrDefaultAsync();
    return item is not null ? Results.Ok(item) : Results.NotFound();
});


app.MapDelete("/api/products/{id}", async (string id) =>
{
    var result = await products.DeleteOneAsync(x => x.Id == id);
    return result.DeletedCount > 0 ? Results.NoContent() : Results.NotFound();
});

app.Run();


public class ProductItem
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }
    public string Name { get; set; } = null!;
    public decimal Price { get; set; }
}

public partial class Program { }