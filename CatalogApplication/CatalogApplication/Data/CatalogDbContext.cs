using CatalogApplication.Models;
using Microsoft.EntityFrameworkCore;

namespace CatalogApplication.Data;

public class CatalogDbContext :DbContext
{
    public DbSet<Catalog> Catalogs { get; set; }
     public CatalogDbContext(DbContextOptions<CatalogDbContext> options) : base(options)
    {
        Database.EnsureCreated();
    }

     protected override void OnModelCreating(ModelBuilder modelBuilder)
     {
         base.OnModelCreating(modelBuilder);
         var catalog1 = new Catalog()
         {
             Id=1,
             Name = "Creating Digital Images"
         };
         var catalog2 = new Catalog()
         {
             Id=2,
             Name = "Resources",
             ParentId = 1
         };
         var catalog3 = new Catalog()
         {
             Id=3,
             Name = "Evidence",
             ParentId = 1
         };
         var catalog4 = new Catalog()
         {
             Id=4,
             Name = "Graphic Products",
             ParentId = 1
         };
         var catalog5 = new Catalog()
         {
             Id=5,
             Name = "Primary Sources",
             ParentId = 2
         };
         var catalog6 = new Catalog()
         {
             Id=6,
             Name = "Secondary Sources",
             ParentId = 2
         };
         var catalog7 = new Catalog()
         {
             Id=7,
             Name = "Process",
             ParentId = 4
         };
         var catalog8 = new Catalog()
         {
             Id=8,
             Name = "Final Product",
             ParentId = 4
         };
         
         modelBuilder.Entity<Catalog>().HasData(new List<Catalog>(){catalog1,catalog2,catalog4});
         modelBuilder.Entity<Catalog>()
             .HasData(new List<Catalog>() { catalog3, catalog5, catalog6, catalog7, catalog8 });
     }
}