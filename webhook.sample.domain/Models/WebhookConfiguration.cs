namespace webhook.sample.domain.Models;

public record WebhookConfiguration(
    string Name,
    string Path,
    string HttpMethod,
    bool IsEnabled,
    string RequestSchemaJson,
    string ResponseSchemaJson,
    string ResponseBodyJson,
    int ResponseStatusCode = 200);

public record WebhookConfigurationView(Guid Id, WebhookConfiguration Configuration);