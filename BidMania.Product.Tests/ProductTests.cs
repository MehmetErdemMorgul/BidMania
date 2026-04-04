using Microsoft.AspNetCore.Mvc.Testing;
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
        _client.DefaultRequestHeaders.Add("X-Internal-Key", InternalKey);

        var response = await _client.GetAsync("/api/products");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Varolan_Urun_Silinmek_Istendiginde_204_NoContent_Donmeli()
    {
        _client.DefaultRequestHeaders.Clear();
        _client.DefaultRequestHeaders.Add("X-Internal-Key", InternalKey);

        var tempProduct = new { Name = "Silinecek", Price = 10.0m };
        var postResponse = await _client.PostAsJsonAsync("/api/products", tempProduct);
        var created = await postResponse.Content.ReadFromJsonAsync<ProductItem>();

        var deleteResponse = await _client.DeleteAsync($"/api/products/{created?.Id}");
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
    }

    [Fact]
    public async Task Varolan_Urun_Guncellenmek_Istendiginde_204_NoContent_Donmeli()
    {
        
        _client.DefaultRequestHeaders.Clear();
        _client.DefaultRequestHeaders.Add("X-Internal-Key", InternalKey);

        var originalItem = new { Name = "Eski Ürün", Price = 100m };
        var postResponse = await _client.PostAsJsonAsync("/api/products", originalItem);
        var created = await postResponse.Content.ReadFromJsonAsync<ProductItem>();

        
        var updatedItem = new ProductItem
        {
            Id = created?.Id,
            Name = "Güncel Ürün",
            Price = 200m
        };

        
        var putResponse = await _client.PutAsJsonAsync($"/api/products/{created?.Id}", updatedItem);

        
        Assert.Equal(HttpStatusCode.NoContent, putResponse.StatusCode);
    }
}