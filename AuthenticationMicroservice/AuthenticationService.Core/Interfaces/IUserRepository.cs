namespace AuthenticationService.Core.Interfaces;

public interface IUserRepository
{
    public Task<User> Login(string username, string password);
}