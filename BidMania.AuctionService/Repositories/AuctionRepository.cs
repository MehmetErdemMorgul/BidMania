using BidMania.AuctionService.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace BidMania.AuctionService.Repositories;

public class AuctionRepository
{
    private readonly IMongoCollection<Auction> _collection;

    public AuctionRepository(IConfiguration configuration, IOptions<AuctionDatabaseSettings> settings)
    {
        var connectionString = configuration["AuctionDatabaseSettings:ConnectionString"]
                               ?? "mongodb://localhost:27017";

        var dbName = settings.Value?.DatabaseName ?? "AuctionDb";
        var colName = settings.Value?.CollectionName ?? "Auctions";

        var client = new MongoClient(connectionString);
        var database = client.GetDatabase(dbName);
        _collection = database.GetCollection<Auction>(colName);
    }

    public async Task CreateAsync(Auction auction) =>
        await _collection.InsertOneAsync(auction);

    public async Task<List<Auction>> GetAllAsync() =>
        await _collection.Find(_ => true).ToListAsync();

    public async Task<Auction?> GetByIdAsync(string id) =>
        await _collection.Find(x => x.Id == id).FirstOrDefaultAsync();
}