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
}