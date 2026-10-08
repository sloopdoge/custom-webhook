using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using MintWebhook.Data;
using webhook.sample.Components;
using webhook.sample.Data;
using webhook.sample.Models;
using webhook.sample.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents().AddInteractiveServerComponents();

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower;
    options.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.SnakeCaseLower;
});
builder.Services.AddDbContext<WebhookDbContext>(options => options.UseSqlite(
    builder.Configuration.GetConnectionString("Webhooks") ?? "Data Source=webhooks.db"));
builder.Services.AddScoped<IWebhookConfigurationService, WebhookConfigurationService>();
builder.Services.AddScoped<IWebhookExecutionService, WebhookExecutionService>();
builder.Services.AddSingleton<WebhookSchemaValidator>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
}

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

await using (var scope = app.Services.CreateAsyncScope())
{
    var database = scope.ServiceProvider.GetRequiredService<WebhookDbContext>();
    await database.Database.MigrateAsync();
    await DefaultWebhookSeeder.SeedAsync(database);
}

app.MapControllers();

app.MapControllers();

app.Run();
