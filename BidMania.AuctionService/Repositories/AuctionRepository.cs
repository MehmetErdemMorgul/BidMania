using BidMania.AuctionService.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace BidMania.AuctionService.Repositories;

public class AuctionRepository
{
    private readonly IMongoCollection<Auction> _collection;

    public AuctionRepository(IOptions<AuctionDatabaseSettings> settings)
    {
        // Mongo Bağlantısı oluşturur 
        var client = new MongoClient(settings.Value.ConnectionString);
        var database = client.GetDatabase(settings.Value.DatabaseName);
        _collection = database.GetCollection<Auction>(settings.Value.CollectionName);
    }

    // Yeni bir açık artırma oluşturur
    public async Task CreateAsync(Auction auction) =>
        await _collection.InsertOneAsync(auction);
    // Tüm açık artırmaları getirir
    public async Task<List<Auction>> GetAllAsync() =>
        await _collection.Find(_ => true).ToListAsync();
    // Belirli bir açık artırmayı ID'sine göre getirir
    public async Task<Auction?> GetByIdAsync(string id) =>
        await _collection.Find(x => x.Id == id).FirstOrDefaultAsync();

}