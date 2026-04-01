namespace BidMania.AuctionService.DTOs;

public record CreateAuctionDto(
    string ItemName,
    decimal StartingPrice,
    DateTime EndTime
);