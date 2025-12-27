using Microsoft.EntityFrameworkCore;
using StockService.Core.Entities;

namespace StockService.Infrastructure;

public class StockDbContext :  DbContext
{
    public StockDbContext(DbContextOptions<StockDbContext> options) : base(options) { }

    public DbSet<Stock>  Stocks { get; set; }
    public DbSet<Product> Products { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Stock>(ent =>
        {
            ent.HasKey(s => s.Id);

            ent.Property(s => s.Id).ValueGeneratedOnAdd();

            ent.Property(s => s.Quantity)
                .IsRequired();

            ent.HasOne(s => s.Product)
                .WithOne()
                .HasForeignKey<Stock>(s => s.ProductId);
        });

        modelBuilder.Entity<Product>(ent =>
        {
            ent.HasKey(p => p.Id);

            ent.Property(p => p.Id).ValueGeneratedOnAdd();

            ent.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(50);

            ent.Property(p => p.Description)
                .HasMaxLength(200);

            ent.Property(p => p.Price)
                .IsRequired();
        });
    }
}