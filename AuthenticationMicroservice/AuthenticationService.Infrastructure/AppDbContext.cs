using Microsoft.EntityFrameworkCore;

namespace AuthenticationService.Infrastructure;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    
    public DbSet<User> Users { get; set; }
    public DbSet<Credential> Credentials { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(ent =>
        {
            ent.HasKey(e => e.Id);
            ent.Property(e => e.Id).ValueGeneratedOnAdd();
            ent.Property(e => e.Username).IsRequired();
        });
        
        modelBuilder.Entity<Credential>(ent =>
        {
            ent.HasKey(c => c.Id);
            
            ent.HasOne<User>()      
                .WithMany()          
                .HasForeignKey(c => c.UserId) 
                .OnDelete(DeleteBehavior.Cascade); 
        });

    }

}