using System.Text.Json.Serialization;

namespace webhook.sample.Models;

public class ExternalLogInResponse
{
    [JsonPropertyName("authorized")]
    public bool Authorized { get; set; }

    [JsonPropertyName("courier_id")]
    public string? CourierId { get; set; }
}
