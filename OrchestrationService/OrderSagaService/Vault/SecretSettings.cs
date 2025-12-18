using Newtonsoft.Json;

namespace OrderSagaService.Vault;

public class SecretSettings
{
    [JsonProperty("rmquser")]
    public string? RmqUser { get; set; }

    [JsonProperty("rmqpassword")]
    public string? RmqPassword { get; set; }

}