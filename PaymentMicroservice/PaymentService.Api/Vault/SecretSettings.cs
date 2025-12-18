using Newtonsoft.Json;

namespace PaymentService.Vault;

public class SecretSettings
{
    [JsonProperty("rmquser")]
    public string? RmqUser { get; set; }

    [JsonProperty("rmqpassword")]
    public string? RmqPassword { get; set; }
    
    [JsonProperty("payment-db")]
    public string? PaymentDb { get; set; }
}