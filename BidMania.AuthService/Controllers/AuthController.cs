using Microsoft.AspNetCore.Mvc;
using BidMania.AuthService.Models;

namespace BidMania.AuthService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    [HttpPost("register")]
    public IActionResult Register([FromBody] UserDto user)
    {
        return Created("", user);
    }
}