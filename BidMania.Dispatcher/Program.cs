using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Yarp.ReverseProxy.Transforms;

var builder = WebApplication.CreateBuilder(args);

// --- YARP AYARLARI ---
var proxyConfig = builder.Configuration.GetSection("ReverseProxy");

builder.Services.AddReverseProxy()
    .LoadFromConfig(proxyConfig)
    .AddTransforms(builderContext =>
    {
        builderContext.AddRequestHeader("X-Internal-Key", "Kocaeli41_Secret");
    });

// --- JWT AYARLARI ---
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("KocaeliBilisimSistemleriMuh41_2026"))
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Default", policy => policy.RequireAuthenticatedUser());
});

builder.Services.AddControllers();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();


try
{
    app.MapReverseProxy();
}
catch (Exception ex)
{
    Console.WriteLine($"!!! KRİTİK HATA: YARP başlatılamadı! Mesaj: {ex.Message}");
}

app.MapControllers();
app.Run();