using System.Text.Json.Serialization;
using Vault;
using Vault.Client;
using Vault.Model;

namespace AuthenticationService;

public static class VaultHelper
{
    public static SecretSettings FetchSecretsFromVault(string vaultHostName, string username, string password)
    {
        Console.WriteLine("Fetching secrets from Vault...");
        var config = new VaultConfiguration(vaultHostName);
        var vaultClient = new VaultClient(config);
        
        Console.WriteLine("Authenticating to Vault...");
        var authResponse = vaultClient.Auth.UserpassLogin(username, new UserpassLoginRequest(password));
        vaultClient.SetToken(authResponse.ResponseAuth.ClientToken);

        Console.WriteLine("Reading secret from Vault...");
        VaultResponse<KvV2ReadResponse> response = vaultClient.Secrets.KvV2Read("secret", "bulls");
        var rawData = response.Data.Data;
        
        // Convert to JSON string (handle JsonElement specially)
        string json;
        
        if (rawData is System.Text.Json.JsonElement je)
        {
            json = je.GetRawText();
        }
        else
        {
            json = System.Text.Json.JsonSerializer.Serialize(rawData);
        }

        var options = new System.Text.Json.JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var settings = System.Text.Json.JsonSerializer.Deserialize<SecretSettings>(json, options)
                       ?? throw new InvalidOperationException("Failed to deserialize Vault secret to SecretSettings.");

        return settings;
    }
}