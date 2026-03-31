using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BidMania.AuctionService.Models;

public class Auction
{
    [BsonId] // Bu alanın MongoDB'deki benzersiz anahtar (Primary Key) olduğunu söyler
    [BsonRepresentation(BsonType.ObjectId)] // Mongo'nun 24 karakterli meşhur Id formatını kullanır
    public string? Id { get; set; }

    public string ItemName { get; set; } = null!;
    public decimal StartingPrice { get; set; }
    public DateTime EndTime { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // İhale ne zaman açıldı?
}