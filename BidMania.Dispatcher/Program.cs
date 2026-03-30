var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddAuthentication("Bearer").AddJwtBearer();
builder.Services.AddAuthorization();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/api/products", () => Results.Ok("Ürünler listelendi"))
   .RequireAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }