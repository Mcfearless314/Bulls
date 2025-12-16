using Microsoft.EntityFrameworkCore;

namespace AuthenticationService.Infrastructure;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

}