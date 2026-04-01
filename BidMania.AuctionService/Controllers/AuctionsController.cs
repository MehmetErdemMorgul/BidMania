using BidMania.AuctionService.Models;
using BidMania.AuctionService.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace BidMania.AuctionService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuctionsController : ControllerBase
{
    // 1. EKSİK: Sınıfın repository'yi tanıması için buraya bir "alan" (field) ekledik
    private readonly AuctionRepository _repository;

    // 2. EKSİK: Constructor (Yapıcı Metot). 
    // Bu sayede Program.cs'deki MongoDB ayarlarını bu sınıfa bağlıyoruz.
    public AuctionsController(AuctionRepository repository)
    {
        _repository = repository;
    }

    [HttpPost]
    public async Task<IActionResult> CreateAuction([FromBody] AuctionDto auctionDto)
    {
        // Validasyonlar (Testlerin geçmesi için burası şart)
        if (string.IsNullOrWhiteSpace(auctionDto.ItemName) ||
            auctionDto.StartingPrice <= 0 ||
            auctionDto.EndTime <= DateTime.Now)
        {
            return BadRequest();
        }

        // DTO'yu gerçek Auction modeline çevirip MongoDB'ye gönderiyoruz
        var newAuction = new Auction
        {
            ItemName = auctionDto.ItemName,
            StartingPrice = auctionDto.StartingPrice,
            EndTime = auctionDto.EndTime
        };

        await _repository.CreateAsync(newAuction); // Gerçekten veri tabanına yazıyoruz!

        return Created("", newAuction);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Auction>>> GetAuctions()
    {
        // Artık _repository burada tanımlı olduğu için hata vermeyecek
        var auctions = await _repository.GetAllAsync();
        return Ok(auctions);
    }
}

// Senin yazdığın DTO burada kalabilir, hiçbir mahsuru yok
public record AuctionDto(string ItemName, decimal StartingPrice, DateTime EndTime);