using Microsoft.EntityFrameworkCore;
using PaymentService.Core.Entities;

namespace PaymentService.Infrastructure;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Payment> Payments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Payment>(ent =>
        {
            ent.HasKey(p => p.Id);

            ent.Property(p => p.Id).ValueGeneratedOnAdd();
            ent.Property(p => p.OrderId).IsRequired();
            ent.Property(p => p.UserId).IsRequired();
            ent.Property(p => p.Amount).IsRequired();
            ent.Property(p => p.Status).IsRequired();
        });
    }
}