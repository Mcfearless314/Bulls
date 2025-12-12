using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace AuthenticationService.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly JwtTokenService _jwtTokenService;

    public AuthController(JwtTokenService jwtTokenService)
    {
        _jwtTokenService = jwtTokenService;
    }

    [HttpPost("login")]
    public IActionResult Login()
    {
        AuthenticationToken token;
        
        try
        {
            token = _jwtTokenService.CreateToken();
        }
        catch (Exception exception)
        {
            return Unauthorized("Token generation failed, please login first.");
        }
        
        return Ok(token);
    }
}