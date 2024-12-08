using MarketplaceBackend.Presentation.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<AnnoucementDb>(opt => opt.UseNpgsql(builder.Configuration.GetConnectionString("DatabaseConnection")));
builder.Services.AddDbContext<AnnoucementDb>(opt => opt.UseInMemoryDatabase("AnnoucementsList"));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
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

//Swagger middleware
if(app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Marketplace API v1");
    });
}

//app.Urls.Add("https://localhost:4000");

//Endpoints
app.MapGet("/", () => "Server works!");

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

//app.MapControllers();
app.Run();

/// Autoryzacja za pomoc¹ JWT
/// Onion architecture (struktura projektu), biblioteka Mediatr
/// Entity framework core v7 (do po³¹czenia z baz¹)
/// Do walidacji biblioteka FluentValidation
/// Jak dodaæ Swagger
/// 
/// Baza PostGres (mo¿liwe korzystanie z Docker :((()
/// Relacje 1->1, 1->Wielu, Wiele->Wielu
/// Indeksy do kolumn
/// Wysy³aæ daty w formacie ISO, daty UTC, lepiej DateTimeOffset
/// 
/// Postman do ³¹czenia siê z API
