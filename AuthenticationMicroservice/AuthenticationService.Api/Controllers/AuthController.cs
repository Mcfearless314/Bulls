using AuthenticationService.Api.DTOs;
using AuthenticationService.Application.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace AuthenticationService.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly JwtTokenService _jwtTokenService;
    private readonly UserService _userService;

    public AuthController(JwtTokenService jwtTokenService, UserService userService)
    {
        _jwtTokenService = jwtTokenService;
        _userService = userService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] CredentialsDto dto)
    {
        AuthenticationToken token;
        
        var user = await _userService.Login(dto.Username, dto.Password);
        if (user == null)
        {
            return BadRequest("Invalid username or password");
        }

        try
        {
            token = _jwtTokenService.CreateToken(user);
        }
        catch (Exception exception)
        {
            return Unauthorized("Token generation failed, please login first.");
        }
        
        return Ok(token);
    }
}