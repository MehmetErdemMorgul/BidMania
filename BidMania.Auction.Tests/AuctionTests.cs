using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Json;
using Xunit;
using BidMania.AuctionService;

namespace BidMania.Auction.Tests;

public class AuctionTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public AuctionTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Yeni_Ihale_Olusturulursa_201_Created_Donmeli()
    {
        var newAuction = new
        {
            ItemName = "Antika Saat",
            StartingPrice = 1000,
            EndTime = DateTime.Now.AddDays(1)
        };

        var response = await _client.PostAsJsonAsync("/api/auctions", newAuction);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }
    [Fact]
    public async Task Fiyat_Sifir_Veya_Negatifse_400_BadRequest_Donmeli()
    {
        var badAuction = new
        {
            ItemName = "Hatalı Ürün",
            StartingPrice = -500,
            EndTime = DateTime.Now.AddDays(1)
        };

        var response = await _client.PostAsJsonAsync("/api/auctions", badAuction);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
    [Fact]
    public async Task Bitis_Tarihi_Gecmisse_400_BadRequest_Donmeli()
    {
        var pastAuction = new
        {
            ItemName = "Eski Eşya",
            StartingPrice = 500,
            EndTime = DateTime.Now.AddDays(-1)
        };

        var response = await _client.PostAsJsonAsync("/api/auctions", pastAuction);

        // 
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    } }