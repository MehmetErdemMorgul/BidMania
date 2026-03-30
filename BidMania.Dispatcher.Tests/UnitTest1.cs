using BidMania.Dispatcher; 
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using Xunit;

namespace BidMania.Dispatcher.Tests;

public class AuthTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public AuthTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
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
        _client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "bu-bir-test-tokenidir");

        var response = await _client.GetAsync("/api/products");

 
        Assert.NotEqual(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
    }
}