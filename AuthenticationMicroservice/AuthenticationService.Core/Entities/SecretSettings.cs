using Newtonsoft.Json;

namespace AuthenticationService.Core.Entities;

public class SecretSettings
{
      [JsonProperty("BullsToken")]
      public string? BullsToken { get; set; }
      
      [JsonProperty("rmquser")]
      public string? RmqUser { get; set; }
      
      [JsonProperty("rmqpassword")]
      public string? RmqPassword { get; set; }
      
      [JsonProperty("auth-db")]
      public string? AuthDb { get; set; }
    
      [JsonProperty("stock-db")]
      public string? StockDb { get; set; }
      
      [JsonProperty("order-db")]
      public string? OrderDb { get; set; }
      
      [JsonProperty("payment-db")]
      public string? PaymentDb { get; set; }
}