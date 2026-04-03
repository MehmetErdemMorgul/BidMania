using BidMania.AuctionService.Models;
using BidMania.AuctionService.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace BidMania.AuctionService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuctionsController : ControllerBase
{
    
    private readonly AuctionRepository _repository;

    
    public AuctionsController(AuctionRepository repository)
    {
        _repository = repository;
    }

    [HttpPost]
    public async Task<IActionResult> CreateAuction([FromBody] AuctionDto auctionDto)
    {
        if (string.IsNullOrWhiteSpace(auctionDto.ItemName) ||
            auctionDto.StartingPrice <= 0 ||
            auctionDto.EndTime <= DateTime.Now)
        {
            return BadRequest();
        }

        
        var newAuction = new Auction
        {
            ItemName = auctionDto.ItemName,
            StartingPrice = auctionDto.StartingPrice,
            EndTime = auctionDto.EndTime
        };

        await _repository.CreateAsync(newAuction); 
        return Created("", newAuction);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Auction>>> GetAuctions()
    {
       
        var auctions = await _repository.GetAllAsync();
        return Ok(auctions);
    }

    [HttpGet("{id}")] 
    public async Task<ActionResult<Auction>> GetAuctionById(string id)
    {
        var auction = await _repository.GetByIdAsync(id);

        
        if (auction == null)
        {
            return NotFound();
        }

        return Ok(auction);
    }
}


public record AuctionDto(string ItemName, decimal StartingPrice, DateTime EndTime);