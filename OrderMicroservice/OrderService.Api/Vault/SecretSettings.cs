using Newtonsoft.Json;

namespace OrderService.Vault;

public class SecretSettings
{
    [JsonProperty("rmquser")]
    public string? RmqUser { get; set; }

    [JsonProperty("rmqpassword")]
    public string? RmqPassword { get; set; }

    [JsonProperty("order-db")]
    public string? OrderDb { get; set; }
}