using Microsoft.AspNetCore.Mvc;
using BidMania.AuthService.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BidMania.AuthService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private const string SecretKey = "KocaeliBilisimSistemleriMuh41_2026";

    [HttpPost("register")]
    public IActionResult Register([FromBody] UserDto user)
    {
        if (string.IsNullOrEmpty(user.Username) || string.IsNullOrEmpty(user.Password))
        {
            return BadRequest();
        }
        return Created("", user);
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] UserDto login)
    {
        if (login.Username == "mustafa" && login.Password == "Password123!")
        {
            var token = GenerateToken(login.Username);
            return Ok(new { token });
        }
        return Unauthorized();
    }

    private string GenerateToken(string username)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            claims: new[] { new Claim(ClaimTypes.Name, username) },
            expires: DateTime.Now.AddMinutes(30),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}