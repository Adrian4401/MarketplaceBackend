using MarketplaceBackend.Presentation.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddDbContext<AnnoucementDb>(opt => opt.UseNpgsql(builder.Configuration.GetConnectionString("DatabaseConnection")));
builder.Services.AddDbContext<AnnoucementDb>(opt => opt.UseInMemoryDatabase("AnnoucementsList"));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

//JWT
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings.GetValue<string>("SecretKey");
var issuer = jwtSettings.GetValue<string>("Issuer");
var audience = jwtSettings.GetValue<string>("Audience");

//CORS policy
builder.Services.AddCors(opt =>
{
    opt.AddDefaultPolicy(policy =>
    {
        policy
        .AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader();
    });
});

//Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Marketplace API",
        Version = "v1",
        Description = "API for marketplace app",
        Contact = new OpenApiContact
        {
            Name = "Test"
        }
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AnnoucementDb>();
    dbContext.Database.Migrate();
}

//Swagger middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Marketplace API v1");
    });
}

//Endpoints
app.MapGet("/", () => "Server works!");

app.MapPost("/login", () =>
{
    // Parametry tokenu JWT
    var claims = new[]
    {
        new Claim(ClaimTypes.Name, "SampleUser"),
        new Claim(ClaimTypes.Role, "User")
    };

    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
    var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

    var token = new JwtSecurityToken(
        issuer,
        audience,
        claims,
        expires: DateTime.Now.AddMinutes(30),
        signingCredentials: credentials
    );

    var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);

    return Results.Ok(new { token = jwtToken });
});

app.MapGet("/annoucements", async (AnnoucementDb db) => 
    await db.Annoucements.ToListAsync());

app.MapGet("/annoucement/{id}", async(int id, AnnoucementDb db) => 
    await db.Annoucements.FindAsync(id) 
        is Annoucement annoucement
            ? Results.Ok(annoucement)
            : Results.NotFound());

app.MapPost("/annoucement-add", async (Annoucement annoucement, AnnoucementDb db) =>
{
    if(string.IsNullOrWhiteSpace(annoucement.Title))
    {
        return Results.BadRequest("Title is required");
    }

    db.Annoucements.Add(annoucement);
    await db.SaveChangesAsync();

    return Results.Created($"/annoucements/{annoucement.Id}", annoucement);
});

app.Urls.Add("http://localhost:4000");
app.UseCors();
app.MapControllers();
app.Run();