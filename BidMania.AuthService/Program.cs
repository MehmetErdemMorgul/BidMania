using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Prometheus;
using BidMania.AuthService.Models;

var builder = WebApplication.CreateBuilder(args);


var isDocker = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true";
var mongoHost = isDocker ? "bidmania_mongo" : "localhost";
var connectionString = $"mongodb://{mongoHost}:27017";
var mongoClient = new MongoClient(connectionString);


var productDb = mongoClient.GetDatabase("ProductDb");
var authDb = mongoClient.GetDatabase("AuthDb");
var productsCollection = productDb.GetCollection<Product>("Products");


builder.Services.AddSingleton<IMongoDatabase>(authDb);
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

app.MapGet("/api/products", async () => await productsCollection.Find(_ => true).ToListAsync());
app.MapPost("/api/products", async (Product product) =>
{
    await productsCollection.InsertOneAsync(product);
    return Results.Created($"/api/products/{product.Id}", product);
});

app.MapControllers(); 
app.MapMetrics();     
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