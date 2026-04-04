using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient();

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
    options.AddPolicy("YazLabAuth", policy => policy.RequireAuthenticatedUser());
});

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.Map("{**slug}", async (string slug, HttpContext context, IHttpClientFactory clientFactory, IConfiguration config) =>
{
    var segments = slug.Split('/');
    if (segments.Length < 2) return;

    var serviceName = segments[1];
    var serviceUrls = config.GetSection("ServiceUrls");
    var targetBaseUrl = serviceUrls[serviceName];

    if (string.IsNullOrEmpty(targetBaseUrl))
    {
        context.Response.StatusCode = 404;
        return;
    }

    if (serviceName != "auth")
    {
        var authResult = await context.AuthenticateAsync(JwtBearerDefaults.AuthenticationScheme);
        if (!authResult.Succeeded)
        {
            context.Response.StatusCode = 401;
            return;
        }
    }

    var client = clientFactory.CreateClient();
    var targetPath = $"{targetBaseUrl}/{slug}{context.Request.QueryString}";

    var requestMessage = new HttpRequestMessage(new HttpMethod(context.Request.Method), targetPath);
    requestMessage.Headers.Add("X-Internal-Key", "Kocaeli41_Secret");

    if (context.Request.ContentLength > 0)
    {
        requestMessage.Content = new StreamContent(context.Request.Body);
        if (context.Request.ContentType != null)
            requestMessage.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(context.Request.ContentType);
    }

    var response = await client.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead);

    context.Response.StatusCode = (int)response.StatusCode;

    foreach (var header in response.Headers)
    {
        if (header.Key.Equals("Transfer-Encoding", StringComparison.OrdinalIgnoreCase) ||
            header.Key.Equals("Connection", StringComparison.OrdinalIgnoreCase)) continue;

        context.Response.Headers[header.Key] = header.Value.ToArray();
    }

    foreach (var header in response.Content.Headers)
    {
        if (header.Key.Equals("Transfer-Encoding", StringComparison.OrdinalIgnoreCase)) continue;
        context.Response.Headers[header.Key] = header.Value.ToArray();
    }

    await response.Content.CopyToAsync(context.Response.Body);
});

Console.WriteLine("MANUEL DISPATCHER AYAKTA");
app.Run();

public partial class Program { }