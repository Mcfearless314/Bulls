using AuthenticationService.Core.Interfaces;

namespace AuthenticationService;

public class UserService
{
    private readonly IUserRepository _userRepository;
    
    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<User> Login(string username, string password)
    {
        return await _userRepository.Login(username, password);
    }
}