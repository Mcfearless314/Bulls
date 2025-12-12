using AuthenticationService.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace AuthenticationService.Controllers;

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
    public IActionResult PostCredentials([FromBody] VaultCredentialsDto credentials)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var vaultHostName = _configuration.GetValue<string>("Settings:VaultHostName") 
                            ?? throw new InvalidOperationException(
                                "Configuration key 'Settings:VaultHostName' is missing.");
        try
        {
            var fetched = VaultHelper.FetchSecretsFromVault(vaultHostName, credentials.Username, credentials.Password);

            _secretSettings.BullsToken = fetched.BullsToken;

            return Ok(new { message = "Secrets fetched and applied." });
        }
        catch (Exception exception)
        {
            return StatusCode(500, new { error = "Failed to fetch secrets from Vault.", details = exception.Message });
        }
    }
}