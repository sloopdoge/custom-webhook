using Microsoft.EntityFrameworkCore;
using webhook.sample.Data;
using webhook.sample.Models;

namespace webhook.sample.Services;

public class WebhookConfigurationService(WebhookDbContext database, WebhookSchemaValidator validator)
    : IWebhookConfigurationService
{
    private static readonly HashSet<string> SupportedMethods = ["GET", "POST", "PUT", "PATCH", "DELETE"];

    public async Task<IReadOnlyList<WebhookConfigurationView>> ListAsync(CancellationToken cancellationToken = default)
    {
        var items = await database.Webhooks.AsNoTracking().OrderBy(x => x.Path).ToListAsync(cancellationToken);
        return items.Select(ToView).ToArray();
    }

    public async Task<WebhookConfigurationView?> GetAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var item = await database.Webhooks.AsNoTracking().SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
        return item is null ? null : ToView(item);
    }

    public async Task<WebhookConfigurationView> CreateAsync(WebhookConfiguration configuration, CancellationToken cancellationToken = default)
    {
        var item = new WebhookDefinition { Id = Guid.NewGuid() };
        await ApplyAsync(item, configuration, cancellationToken);
        database.Webhooks.Add(item);
        await database.SaveChangesAsync(cancellationToken);
        return ToView(item);
    }

    public async Task<WebhookConfigurationView?> UpdateAsync(Guid id, WebhookConfiguration configuration, CancellationToken cancellationToken = default)
    {
        var item = await database.Webhooks.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (item is null) return null;
        await ApplyAsync(item, configuration, cancellationToken);
        await database.SaveChangesAsync(cancellationToken);
        return ToView(item);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await database.Webhooks.Where(x => x.Id == id).ExecuteDeleteAsync(cancellationToken) > 0;
    }

    private async Task ApplyAsync(WebhookDefinition item, WebhookConfiguration config, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(config);
        ArgumentException.ThrowIfNullOrWhiteSpace(config.Name);
        ArgumentException.ThrowIfNullOrWhiteSpace(config.Path);
        ArgumentException.ThrowIfNullOrWhiteSpace(config.HttpMethod);
        var path = NormalizePath(config.Path);
        var method = config.HttpMethod.Trim().ToUpperInvariant();
        if (!SupportedMethods.Contains(method)) throw new ArgumentException("Unsupported HTTP method.");
        // Every configured response includes a JSON body.
        if (config.ResponseStatusCode is < 200 or > 599 or 204 or 205 or 304)
            throw new ArgumentException("Status code must allow a JSON response body.");
        validator.ValidateConfiguration(config.RequestSchemaJson, config.ResponseSchemaJson, config.ResponseBodyJson);
        if (await database.Webhooks.AnyAsync(x => x.Id != item.Id && x.Path == path && x.HttpMethod == method, cancellationToken))
            throw new InvalidOperationException("A webhook already exists for this path and method.");

        item.Name = config.Name.Trim();
        item.Path = path;
        item.HttpMethod = method;
        item.IsEnabled = config.IsEnabled;
        item.RequestSchemaJson = config.RequestSchemaJson;
        item.ResponseSchemaJson = config.ResponseSchemaJson;
        item.ResponseBodyJson = config.ResponseBodyJson;
        item.ResponseStatusCode = config.ResponseStatusCode;
    }

    public static string NormalizePath(string path)
    {
        path = path.Trim();
        if (path.Contains('?') || path.Contains('#') || path.Contains('{') || path.Contains('}') ||
            path.Contains("://") || path.Contains('\\'))
            throw new ArgumentException("Use a literal URL path without query, fragment or route parameters.");
        return "/" + path.Trim('/').ToLowerInvariant();
    }

    private static WebhookConfigurationView ToView(WebhookDefinition x) => new(x.Id,
        new(x.Name, x.Path, x.HttpMethod, x.IsEnabled, x.RequestSchemaJson,
            x.ResponseSchemaJson, x.ResponseBodyJson, x.ResponseStatusCode));
}
