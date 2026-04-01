using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddHttpClient();
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("KocaeliBilisimSistemleriMuh41_2026"))
        };
    });
builder.Services.AddAuthorization();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/api/products", () => Results.Ok("Ürünler listelendi"))
   .RequireAuthorization();

app.Map("/api/auctions/{**remainder}", async (HttpContext context, HttpClient client) =>
{
    var remainder = context.Request.RouteValues["remainder"]?.ToString() ?? "";
    var targetUrl = $"http://localhost:5183/api/auctions/{remainder}".TrimEnd('/');

    var requestMessage = new HttpRequestMessage(new HttpMethod(context.Request.Method), targetUrl);

    // Body kopyalama
    if (context.Request.ContentLength > 0)
    {
        requestMessage.Content = new StreamContent(context.Request.Body);
    }

    // --- KRÝTÝK DÜZELTME: Header Yönetimi ---
    foreach (var header in context.Request.Headers)
    {
        // 'Host' header'ýný kopyalamýyoruz, hedef sunucu kendi host'unu kullanmalý
        if (header.Key.Equals("Host", StringComparison.OrdinalIgnoreCase)) continue;

        // Header'ý genel listeye eklemeyi dene, baþaramazsan Content kýsmýna ekle
        if (!requestMessage.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray()))
        {
            requestMessage.Content?.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray());
        }
    }

    try
    {
        var response = await client.SendAsync(requestMessage);
        context.Response.StatusCode = (int)response.StatusCode;

        // Cevap header'larýný kopyala
        foreach (var header in response.Headers)
        {
            context.Response.Headers[header.Key] = header.Value.ToArray();
        }
        foreach (var header in response.Content.Headers)
        {
            context.Response.Headers[header.Key] = header.Value.ToArray();
        }

        var content = await response.Content.ReadAsByteArrayAsync();
        await context.Response.Body.WriteAsync(content);
    }
    catch (Exception)
    {
        context.Response.StatusCode = 502; // Bad Gateway - Arka servise ulaþýlamadý
    }
}).RequireAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }