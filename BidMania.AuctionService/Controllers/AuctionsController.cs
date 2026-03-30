using Microsoft.AspNetCore.Mvc;

namespace BidMania.AuctionService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuctionsController : ControllerBase
{
    [HttpPost]
    public IActionResult CreateAuction([FromBody] object auction)
    {
        return Created("", auction);
    }
}