namespace AuthenticationService.Core.Entities;

public class Credential
{
    public Guid Id { get; set; }
    public int UserId { get; set; }
    public string Password { get; set; }
}