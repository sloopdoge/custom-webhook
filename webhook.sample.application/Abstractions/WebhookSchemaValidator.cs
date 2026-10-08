using System.Text.Json;
using Json.Schema;

namespace webhook.sample.application.Abstractions;

public class WebhookSchemaValidator
{
    public JsonSchema Parse(string schemaJson)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(schemaJson);
        // Build validates the schema itself; external references are not fetched.
        return JsonSchema.FromText(schemaJson);
    }

    public bool IsValid(JsonSchema schema, JsonElement value) => schema.Evaluate(value).IsValid;

    public void ValidateConfiguration(string requestSchema, string responseSchema, string responseBody)
    {
        Parse(requestSchema);
        var schema = Parse(responseSchema);
        using var body = JsonDocument.Parse(responseBody);
        if (!IsValid(schema, body.RootElement))
            throw new ArgumentException("Response body does not match response schema.");
    }
}