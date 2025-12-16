using AuthenticationService.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AuthenticationService.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<User> Login(string username, string password)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Username == username);

        if (user == null)
            return null;

        var credentials = await _context.Credentials
            .FirstOrDefaultAsync(c => c.UserId == user.Id);

        if (credentials == null)
            return null;

        if (credentials.Password != password)
            return null;
        
        return user;
    }
}