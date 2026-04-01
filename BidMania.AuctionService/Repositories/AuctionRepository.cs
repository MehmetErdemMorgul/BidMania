using BidMania.AuctionService.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace BidMania.AuctionService.Repositories;

public class AuctionRepository
{
    private readonly IMongoCollection<Auction> _collection;

    public AuctionRepository(IOptions<AuctionDatabaseSettings> settings)
    {
        // Appsettings'ten bilgileri çekip Mongo'ya bağlanıyoruz
        var client = new MongoClient(settings.Value.ConnectionString);
        var database = client.GetDatabase(settings.Value.DatabaseName);
        _collection = database.GetCollection<Auction>(settings.Value.CollectionName);
    }

    // Yeni ihale kaydetme metodu
    public async Task CreateAsync(Auction auction) =>
        await _collection.InsertOneAsync(auction);

    // Tüm ihaleleri getirme metodu
    public async Task<List<Auction>> GetAllAsync() =>
        await _collection.Find(_ => true).ToListAsync();


}