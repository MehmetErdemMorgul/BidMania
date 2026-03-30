using BidMania.AuthService;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace BidMania.Auth.Tests;

public class UserTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public UserTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Kayit_Basarili_Olursa_201_Created_Donmeli()
    {
        var user = new { Username = "mustafa", Password = "Password123!" };

        var response = await _client.PostAsJsonAsync("/api/auth/register", user);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task Eksik_Bilgiyle_Kayit_Olunursa_400_BadRequest_Donmeli()
    {
        var incompleteUser = new { Username = "", Password = "" };

        var response = await _client.PostAsJsonAsync("/api/auth/register", incompleteUser);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]

    public async Task Dogru_Bilgilerle_Login_Olunursa_Token_Donmeli()
    {
        var loginDto = new { Username = "mustafa", Password = "Password123!" };

        var response = await _client.PostAsJsonAsync("/api/auth/login", loginDto);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();

        Assert.NotNull(result);
        Assert.True(result.ContainsKey("token"));
        Assert.False(string.IsNullOrEmpty(result["token"]));
    }

    [Fact]
    public async Task Yanlis_Sifreyle_Login_Olunursa_401_Unauthorized_Donmeli()
    {
        var wrongLogin = new { Username = "mustafa", Password = "wrongpassword" };

        var response = await _client.PostAsJsonAsync("/api/auth/login", wrongLogin);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}