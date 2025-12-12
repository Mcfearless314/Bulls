using System.ComponentModel.DataAnnotations;

namespace AuthenticationService.DTOs;

public class VaultCredentialsDto
{
    [Required]
    public string Username { get; set; }

    [Required]
    public string Password { get; set; }
}