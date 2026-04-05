using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Prometheus;

var builder = WebApplication.CreateBuilder(args);

var isDocker = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true";
var mongoHost = isDocker ? "bidmania_mongo" : "localhost";
var connectionString = $"mongodb://{mongoHost}:27017";

var mongoClient = new MongoClient(connectionString);
var database = mongoClient.GetDatabase("ProductDb");
var products = database.GetCollection<ProductItem>("Products");

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

    if (!context.Request.Headers.TryGetValue("X-Internal-Key", out var key) || key != "Kocaeli41_Secret")
    {
        context.Response.StatusCode = 403;
        await context.Response.WriteAsync("Erisim Yasak: Sadece Dispatcher uzerinden gelmelisiniz.");
        return;
    }
    await next();
});

app.MapPost("/api/products", async (ProductItem item) =>
{
    if (string.IsNullOrEmpty(item.Name) || item.Price <= 0)
        return Results.BadRequest();

    await products.InsertOneAsync(item);
    return Results.Created($"/api/products/{item.Id}", item);
});

app.MapGet("/api/products", async () =>
{
    var all = await products.Find(_ => true).ToListAsync();
    return Results.Ok(all);
});

app.MapPut("/api/products/{id}", async (string id, ProductItem updatedItem) =>
{
    var result = await products.ReplaceOneAsync(x => x.Id == id, updatedItem);
    return result.MatchedCount > 0 ? Results.NoContent() : Results.NotFound();
});

app.MapDelete("/api/products/{id}", async (string id) =>
{
    var result = await products.DeleteOneAsync(x => x.Id == id);
    return result.DeletedCount > 0 ? Results.NoContent() : Results.NotFound();
});

app.MapMetrics();

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