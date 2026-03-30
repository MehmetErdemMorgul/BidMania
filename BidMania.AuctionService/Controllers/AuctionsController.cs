using Microsoft.AspNetCore.Mvc;

namespace BidMania.AuctionService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuctionsController : ControllerBase
{
    [HttpPost]
    public IActionResult CreateAuction([FromBody] AuctionDto auction)
    {
        if (auction.StartingPrice <= 0||auction.EndTime <= DateTime.Now)
        {
            return BadRequest();
        }

        return Created("", auction);
    }
}

public record AuctionDto(string ItemName, decimal StartingPrice, DateTime EndTime);