using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System.Net;
using System.Net.Http.Json;
using Xunit;


namespace BidMania.Product.Tests;

public class ProductTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private const string InternalKey = "Kocaeli41_Secret";

    public ProductTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Yeni_Urun_Eklendiginde_201_Created_Donmeli()
    {
        
        var product = new { Name = "Test Ürünü", Price = 100.50m };

        _client.DefaultRequestHeaders.Clear();
        _client.DefaultRequestHeaders.Add("X-Internal-Key", InternalKey);

        var response = await _client.PostAsJsonAsync("/api/products", product);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task Urunler_Listelenmek_Istendiginde_200_OK_Donmeli()
    {
        
        _client.DefaultRequestHeaders.Clear();
        _client.DefaultRequestHeaders.Add("X-Internal-Key", "Kocaeli41_Secret");

        
        var response = await _client.GetAsync("/api/products");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}