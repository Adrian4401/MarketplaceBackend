//Database context

using MarketplaceBackend.Presentation.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

public class AnnoucementDb: DbContext
{
    public AnnoucementDb(DbContextOptions<AnnoucementDb> options)
        : base(options) { }

    //public DbSet<Annoucement> Annoucements => Set<Annoucement>();
    public DbSet<Annoucement> Annoucements { get; set; }
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Annoucement>()
            .HasOne<User>()
            .WithMany(u => u.Annoucements)
            .HasForeignKey("UserId");
            // One To Many
    }
}
