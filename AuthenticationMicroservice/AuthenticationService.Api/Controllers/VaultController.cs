using AuthenticationService.Api.DTOs;
using AuthenticationService.Application.Services;
using AuthenticationService.Core.Entities;
using Microsoft.AspNetCore.Mvc;

namespace AuthenticationService.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class VaultController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly SecretSettings _secretSettings;

    public VaultController(IConfiguration configuration, SecretSettings secretSettings)
    {
        _configuration = configuration;
        _secretSettings = secretSettings;
    }

    [HttpPost("credentials")]
    public IActionResult PostCredentials([FromBody] CredentialsDto credentials)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        
        try
        {
            var vaultHostName = _configuration.GetValue<string>("Vault:VaultHostName");
            var vaultPath = _configuration.GetValue<string>("Vault:VaultPath");
            var vaultKvV2MountPath = _configuration.GetValue<string>("Vault:VaultKvV2MountPath");
            
            var fetched = VaultHelper.FetchSecretsFromVault(
                vaultHostName,
                vaultPath,
                vaultKvV2MountPath,
                credentials.Username,
                credentials.Password);

            _secretSettings.BullsToken = fetched.BullsToken;

            return Ok(new { message = "Secrets fetched and applied." });
        }
        catch (Exception exception)
        {
            return StatusCode(500, new { error = "Failed to fetch secrets from Vault.", details = exception.Message });
        }
    }
}