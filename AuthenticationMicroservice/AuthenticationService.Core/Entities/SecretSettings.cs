using Newtonsoft.Json;

namespace AuthenticationService;

public class SecretSettings
{
      [JsonProperty("BullsToken")]
      public string? BullsToken { get; set; }
}