using Newtonsoft.Json.Linq;
using Vault;
using Vault.Client;
using Vault.Model;

namespace AuthenticationService;

public static class VaultHelper
{
    public static SecretSettings? FetchSecretsFromVault(string vaultHostName, string username, string password)
    {
        Console.WriteLine("Fetching secrets from Vault...");
        var config = new VaultConfiguration(vaultHostName);
        var vaultClient = new VaultClient(config);
        
        Console.WriteLine("Authenticating to Vault...");
        var authResponse = vaultClient.Auth.UserpassLogin(username, new UserpassLoginRequest(password));
        vaultClient.SetToken(authResponse.ResponseAuth.ClientToken);

        Console.WriteLine("Reading secret from Vault...");
        VaultResponse<KvV2ReadResponse> response = vaultClient.Secrets.KvV2Read("secret", "bulls");
        
        JObject data = (JObject)response.Data.Data;
        
        return data.ToObject<SecretSettings>();
    }
}