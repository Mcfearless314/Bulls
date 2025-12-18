using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AuthenticationService.Core.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Tokens;

namespace AuthenticationService.Application.Services;

public class JwtTokenService
{
    private readonly SecretSettings _secretSettings;
    
    public JwtTokenService(SecretSettings secretSettings)
    {
        _secretSettings = secretSettings;
    }

    public AuthenticationToken CreateToken(User user)
    {
        var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretSettings.BullsToken));
        var signingCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
        
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim("scope", "bulls_management_service")
        };
        
        var tokenOptions = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.Now.AddHours(12),
            signingCredentials: signingCredentials
        );
        
        
        var tokenString = new JwtSecurityTokenHandler().WriteToken(tokenOptions);
        
        return new AuthenticationToken
        {
            Value = tokenString
        };
    }
}