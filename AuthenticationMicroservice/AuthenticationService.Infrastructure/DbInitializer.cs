using AuthenticationService.Core.Entities;

namespace AuthenticationService.Infrastructure;

public class DbInitializer
{
    private readonly AppDbContext _context;
    
    public DbInitializer(AppDbContext context)
    {
        _context = context;
    }

    public async Task InitializeAsync(AppDbContext context)
    {
        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();

        var user = new User
        {
            Username = "John"
        };
        
        await context.Users.AddAsync(user);
        await context.SaveChangesAsync();

        var credential = new Credential
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Password = "john123"
        };
        await context.Credentials.AddRangeAsync(credential);
        await context.SaveChangesAsync();
    }
}