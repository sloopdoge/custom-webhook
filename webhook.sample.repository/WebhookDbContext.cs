using Microsoft.EntityFrameworkCore;

namespace webhook.sample.repository;

public class WebhookDbContext(DbContextOptions<WebhookDbContext> options) : DbContext(options)
{
    public DbSet<WebhookDefinition> Webhooks => Set<WebhookDefinition>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var webhook = modelBuilder.Entity<WebhookDefinition>();
        webhook.ToTable("webhooks");
        webhook.HasKey(x => x.Id);
        webhook.HasIndex(x => new { x.Path, x.HttpMethod }).IsUnique();
        webhook.Property(x => x.Name).IsRequired();
        webhook.Property(x => x.Path).IsRequired();
        webhook.Property(x => x.HttpMethod).IsRequired();
        webhook.Property(x => x.RequestSchemaJson).IsRequired();
        webhook.Property(x => x.ResponseSchemaJson).IsRequired();
        webhook.Property(x => x.ResponseBodyJson).IsRequired();
    }
}