using Newtonsoft.Json;

namespace StockService.Vault;

public class SecretSettings
{
    [JsonProperty("rmquser")]
    public string? RmqUser { get; set; }

    [JsonProperty("rmqpassword")]
    public string? RmqPassword { get; set; }
    
    [JsonProperty("stock-db")]
    public string? StockDb { get; set; }
}