namespace webhook.sample.repository;

public class WebhookDefinition
{
    public Guid Id { get; set; }
    public string Name { get; set; } = "";
    public string Path { get; set; } = "";
    public string HttpMethod { get; set; } = "POST";
    public bool IsEnabled { get; set; } = true;
    public string RequestSchemaJson { get; set; } = "{}";
    public string ResponseSchemaJson { get; set; } = "{}";
    public string ResponseBodyJson { get; set; } = "{}";
    public int ResponseStatusCode { get; set; } = 200;
}