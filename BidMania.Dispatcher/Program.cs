using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Yarp.ReverseProxy.Transforms; 

var builder = WebApplication.CreateBuilder(args);

// 1. YARP 
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"))
    .AddTransforms(builderContext =>
    {
        builderContext.AddRequestHeader("X-Internal-Key", "Kocaeli41_Secret");
    });

// JWT
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
    options.AddPolicy("DispatcherPolicy", policy => policy.RequireAuthenticatedUser());
});
builder.Services.AddControllers();

var app = builder.Build();


app.UseAuthentication();
app.UseAuthorization();

app.MapReverseProxy(); 
app.MapControllers();

app.Run();

public partial class Program { }