using BidMania.Dispatcher;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using Xunit;
using Xunit.Abstractions;

namespace BidMania.Dispatcher.Tests;

public class AuthTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly ITestOutputHelper _output;
    private const string SecretKey = "KocaeliBilisimSistemleriMuh41_2026";

    public AuthTests(WebApplicationFactory<Program> factory, ITestOutputHelper output)
    {
        _client = factory.CreateClient();
        _output = output;
    }

    [Fact]
    public async Task Token_Gonderilmezse_401_Unauthorized_Donmeli()
    {
        var response = await _client.GetAsync("/api/products");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Gecerli_Token_Gonderilirse_Erisim_Saglanmali()
    {
        var token = GenerateTestToken();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _client.GetAsync("/api/products");

        Assert.NotEqual(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Gecerli_Token_Ile_Auctions_Istegi_Basariyla_Yonlendirilmeli()
    {
        var token = GenerateTestToken();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _client.GetAsync("/api/auctions");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    private string GenerateTestToken()
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            claims: new[] { new Claim(ClaimTypes.Name, "Mustafa") },
            expires: DateTime.Now.AddMinutes(30),
            signingCredentials: credentials);

        var encodedToken = new JwtSecurityTokenHandler().WriteToken(token);

        _output.WriteLine($"POSTMAN_TOKEN: {encodedToken}");

        return encodedToken;
    }

    [Fact]
    public async Task Varolan_Bir_Ihale_ID_Ile_Istenirse_200_OK_Donmeli()
    {
        
        var testId = "66085a69842f61642c67292a";
        var response = await _client.GetAsync($"/api/auctions/{testId}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}