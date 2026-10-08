using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using webhook.sample.application.Interfaces;
using webhook.sample.repository;

namespace webhook.sample.application.Abstractions;

public class WebhookExecutionService(
    WebhookDbContext database,
    WebhookSchemaValidator validator,
    ILogger<WebhookExecutionService> logger) : IWebhookExecutionService
{
    public async Task<WebhookExecutionResult> ExecuteAsync(string path, string method, JsonElement body,
        CancellationToken cancellationToken = default)
    {
        var normalizedPath = WebhookConfigurationService.NormalizePath(path);
        var normalizedMethod = method.ToUpperInvariant();
        var config = await database.Webhooks.AsNoTracking().SingleOrDefaultAsync(
            x => x.Path == normalizedPath && x.HttpMethod == normalizedMethod && x.IsEnabled, cancellationToken);
        if (config is null) return new(404, null, "Webhook not found.");

        if (!validator.IsValid(validator.Parse(config.RequestSchemaJson), body))
        {
            logger.LogWarning("Request rejected by schema for webhook {WebhookId}", config.Id);
            return new(400, null, "Request body does not match request schema.");
        }

        using var response = JsonDocument.Parse(config.ResponseBodyJson);
        if (!validator.IsValid(validator.Parse(config.ResponseSchemaJson), response.RootElement))
        {
            logger.LogError("Invalid response configuration for webhook {WebhookId}", config.Id);
            return new(500, null, "Invalid webhook response configuration.");
        }

        logger.LogInformation("Webhook {WebhookId} handled with status {StatusCode}", config.Id,
            config.ResponseStatusCode);
        return new(config.ResponseStatusCode, config.ResponseBodyJson);
    }
}