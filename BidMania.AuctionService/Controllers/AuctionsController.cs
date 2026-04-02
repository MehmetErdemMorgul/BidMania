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

    [HttpGet("{id}")] // Burada id'ye göre bir ihale getireceğiz, bu yüzden URL'de {id} parametresi var
    public async Task<ActionResult<Auction>> GetAuctionById(string id)
    {
        var auction = await _repository.GetByIdAsync(id);

        //404 Not Found: Eğer böyle bir ihale yoksa, kullanıcıya bunu bildirmek için bu kodu kullanıyoruz
        if (auction == null)
        {
            return NotFound();
        }

        // 200 OK: Eğer ihale bulunduysa, onu kullanıcıya geri gönderiyoruz
        return Ok(auction);
    }
}


public record AuctionDto(string ItemName, decimal StartingPrice, DateTime EndTime);