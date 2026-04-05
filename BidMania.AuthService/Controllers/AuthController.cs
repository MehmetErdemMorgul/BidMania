using Microsoft.AspNetCore.Mvc;
using BidMania.AuthService.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using MongoDB.Driver;

namespace BidMania.AuthService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private const string SecretKey = "KocaeliBilisimSistemleriMuh41_2026";
    private readonly IMongoCollection<User> _users;

    
    public AuthController(IMongoDatabase database)
    {
        _users = database.GetCollection<User>("Users");
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] UserDto userDto)
    {
        if (string.IsNullOrEmpty(userDto.Username) || string.IsNullOrEmpty(userDto.Password))
            return BadRequest("Kullanıcı adı veya şifre boş olamaz.");

        var existingUser = await _users.Find(u => u.Username == userDto.Username).FirstOrDefaultAsync();
        if (existingUser != null)
            return BadRequest("Bu kullanıcı zaten mevcut.");

        var newUser = new User { Username = userDto.Username, Password = userDto.Password };
        await _users.InsertOneAsync(newUser);

        return Created("", new { message = "Kayıt Başarılı", username = userDto.Username });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] UserDto login)
    {
        var user = await _users.Find(u => u.Username == login.Username && u.Password == login.Password).FirstOrDefaultAsync();

        if (user != null)
        {
            var token = GenerateToken(user.Username);
            return Ok(new { token });
        }

        return Unauthorized("Hatalı kullanıcı adı veya şifre.");
    }

    private string GenerateToken(string username)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[] { new Claim(ClaimTypes.Name, username) };

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.Now.AddMinutes(30),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}