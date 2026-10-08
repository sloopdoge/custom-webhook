using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace webhook.sample.repository.Migrations;

[DbContext(typeof(WebhookDbContext))]
[Migration("202610070001_InitialCreate")]
public class InitialCreate : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable("webhooks", columns: table => new
        {
            Id = table.Column<Guid>(type: "TEXT", nullable: false),
            Name = table.Column<string>(type: "TEXT", nullable: false),
            Path = table.Column<string>(type: "TEXT", nullable: false),
            HttpMethod = table.Column<string>(type: "TEXT", nullable: false),
            IsEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
            RequestSchemaJson = table.Column<string>(type: "TEXT", nullable: false),
            ResponseSchemaJson = table.Column<string>(type: "TEXT", nullable: false),
            ResponseBodyJson = table.Column<string>(type: "TEXT", nullable: false),
            ResponseStatusCode = table.Column<int>(type: "INTEGER", nullable: false)
        }, constraints: table => table.PrimaryKey("PK_webhooks", x => x.Id));
        migrationBuilder.CreateIndex("IX_webhooks_Path_HttpMethod", "webhooks",
            new[] { "Path", "HttpMethod" }, unique: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder) => migrationBuilder.DropTable("webhooks");

    protected override void BuildTargetModel(ModelBuilder modelBuilder)
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
