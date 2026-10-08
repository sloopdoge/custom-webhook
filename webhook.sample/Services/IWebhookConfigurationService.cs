using webhook.sample.Models;

namespace webhook.sample.Services;

public interface IWebhookConfigurationService
{
    Task<IReadOnlyList<WebhookConfigurationView>> ListAsync(CancellationToken cancellationToken = default);
    Task<WebhookConfigurationView?> GetAsync(Guid id, CancellationToken cancellationToken = default);
    Task<WebhookConfigurationView> CreateAsync(WebhookConfiguration configuration, CancellationToken cancellationToken = default);
    Task<WebhookConfigurationView?> UpdateAsync(Guid id, WebhookConfiguration configuration, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
