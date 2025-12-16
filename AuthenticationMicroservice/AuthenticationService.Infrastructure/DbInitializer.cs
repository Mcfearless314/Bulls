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

        var users = new List<User>
        {
            new User {Username = "John"},
            new User {Username = "Jane"}
        };
        
        await context.Users.AddRangeAsync(users);
        await context.SaveChangesAsync();

        var credentials = new List<Credential>
        {
            new Credential {Id = Guid.NewGuid(), UserId = 1, Password = "john123"},
            new Credential {Id = Guid.NewGuid(), UserId = 2, Password = "jane123"}
        };
        
        await context.Credentials.AddRangeAsync(credentials);
        await context.SaveChangesAsync();
    }
}