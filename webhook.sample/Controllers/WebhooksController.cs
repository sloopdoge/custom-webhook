using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using webhook.sample.Services;

namespace webhook.sample.Controllers;

[ApiController]
[Route("{**path}")]
public class WebhooksController(IWebhookExecutionService executionService,
    ILogger<WebhooksController> logger) : ControllerBase
{
    [AcceptVerbs("GET", "POST", "PUT", "PATCH", "DELETE")]
    public async Task<IActionResult> Handle(string? path, CancellationToken cancellationToken)
    {
        logger.LogInformation("Received webhook {Method} {Path}", Request.Method, Request.Path);

        // GET and requests without a body are evaluated as JSON null.
        JsonElement body;
        try
        {
            if (Request.ContentLength == 0 || (Request.ContentLength is null && !Request.Headers.ContainsKey("Transfer-Encoding")))
            {
                body = JsonSerializer.SerializeToElement<object?>(null);
            }
            else
            {
                if (!Request.HasJsonContentType())
                    return StatusCode(415, new { error = "Content-Type must be application/json." });
                using var document = await JsonDocument.ParseAsync(Request.Body, cancellationToken: cancellationToken);
                body = document.RootElement.Clone();
            }
        }
        catch (JsonException)
        {
            return BadRequest(new { error = "Invalid JSON body." });
        }

        var result = await executionService.ExecuteAsync("/" + path, Request.Method, body, cancellationToken);
        if (result.Error is not null) return StatusCode(result.StatusCode, new { error = result.Error });

        return new ContentResult
        {
            Content = result.ResponseJson,
            ContentType = "application/json",
            StatusCode = result.StatusCode
        };
    }
}
