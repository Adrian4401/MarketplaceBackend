using MarketplaceBackend.Presentation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.InMemory;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<AnnoucementDb>(opt => opt.UseInMemoryDatabase("AnnoucementsList"));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
var app = builder.Build();

app.MapGet("/annoucements", async (AnnoucementDb db) => 
    await db.Annoucements.ToListAsync());

app.MapGet("/annoucement/{id}", async(int id, AnnoucementDb db) => 
    await db.Annoucements.FindAsync(id) 
        is Annoucement annoucement
            ? Results.Ok(annoucement)
            : Results.NotFound());

app.MapPost("/annoucement-add", async (Annoucement annoucement, AnnoucementDb db) =>
{
    db.Annoucements.Add(annoucement);
    await db.SaveChangesAsync();

    return Results.Created($"/annoucements/{annoucement.Id}", annoucement);
});

app.Run();

/// Autoryzacja za pomoc¹ JWT
/// Onion architecture (struktura projektu), biblioteka Mediatr
/// Entity framework core v7 (do po³¹czenia z baz¹)
/// Baza PostGres (mo¿liwe korzystanie z Docker :((()
/// Do walidacji biblioteka FluentValidation
/// Jak dodaæ Swagger
/// Relacje 1->1, 1->Wielu, Wiele->Wielu
/// Indeksy do kolumn
/// Wysy³aæ daty w formacie ISO, daty UTC, lepiej DateTimeOffset
/// 
/// 
/// Postman do ³¹czenia siê z API
