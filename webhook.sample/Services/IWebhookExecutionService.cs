using System.Text.Json;

namespace webhook.sample.Services;

public record WebhookExecutionResult(int StatusCode, string? ResponseJson, string? Error = null);

public interface IWebhookExecutionService
{
    Task<WebhookExecutionResult> ExecuteAsync(string path, string method, JsonElement body,
        CancellationToken cancellationToken = default);
}
