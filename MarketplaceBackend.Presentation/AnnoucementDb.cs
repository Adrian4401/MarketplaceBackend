//Database context

using MarketplaceBackend.Presentation;
using Microsoft.EntityFrameworkCore;

class AnnoucementDb: DbContext
{
    public AnnoucementDb(DbContextOptions<AnnoucementDb> options)
        : base(options) { }

    public DbSet<Annoucement> Annoucements => Set<Annoucement>();
}
