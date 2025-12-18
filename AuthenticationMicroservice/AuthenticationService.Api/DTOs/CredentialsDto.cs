using System.ComponentModel.DataAnnotations;

namespace AuthenticationService.Api.DTOs;

public class CredentialsDto
{
    [Required]
    public string Username { get; set; }

    [Required]
    public string Password { get; set; }
}