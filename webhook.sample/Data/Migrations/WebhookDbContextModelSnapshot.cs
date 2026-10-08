using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace webhook.sample.Data.Migrations;

[DbContext(typeof(WebhookDbContext))]
public class WebhookDbContextModelSnapshot : ModelSnapshot
{
    protected override void BuildModel(ModelBuilder modelBuilder)
    {
        modelBuilder.HasAnnotation("ProductVersion", "10.0.12");
        modelBuilder.Entity("MintWebhook.Data.WebhookDefinition", entity =>
        {
            entity.Property<Guid>("Id").ValueGeneratedOnAdd().HasColumnType("TEXT");
            entity.Property<string>("Name").IsRequired().HasColumnType("TEXT");
            entity.Property<string>("Path").IsRequired().HasColumnType("TEXT");
            entity.Property<string>("HttpMethod").IsRequired().HasColumnType("TEXT");
            entity.Property<bool>("IsEnabled").HasColumnType("INTEGER");
            entity.Property<string>("RequestSchemaJson").IsRequired().HasColumnType("TEXT");
            entity.Property<string>("ResponseSchemaJson").IsRequired().HasColumnType("TEXT");
            entity.Property<string>("ResponseBodyJson").IsRequired().HasColumnType("TEXT");
            entity.Property<int>("ResponseStatusCode").HasColumnType("INTEGER");
            entity.HasKey("Id");
            entity.HasIndex("Path", "HttpMethod").IsUnique();
            entity.ToTable("webhooks");
        });
    }
}
