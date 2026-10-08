using Microsoft.EntityFrameworkCore;

namespace webhook.sample.repository;

public static class DefaultWebhookSeeder
{
    public static async Task SeedAsync(WebhookDbContext database)
    {
        // Once any configuration exists, seeding does not overwrite it.
        if (await database.Webhooks.AnyAsync()) return;

        database.Webhooks.Add(new WebhookDefinition
        {
            Id = Guid.Parse("978f60ac-7fc9-45c7-ae5f-92f8b21b1d30"),
            Name = "Mint courier authorization",
            Path = "/webhooks/mint/courier-authorization",
            HttpMethod = "POST",
            RequestSchemaJson = """
                                {
                                  "$schema": "https://json-schema.org/draft/2020-12/schema",
                                  "type": "object",
                                  "properties": {
                                    "user_code": { "type": "string", "minLength": 1 },
                                    "pin_code": { "type": "string", "minLength": 1 }
                                  },
                                  "required": ["user_code", "pin_code"],
                                  "additionalProperties": false
                                }
                                """,
            ResponseSchemaJson = """
                                 {
                                   "$schema": "https://json-schema.org/draft/2020-12/schema",
                                   "type": "object",
                                   "properties": {
                                     "authorized": { "type": "boolean" },
                                     "courier_id": { "type": ["string", "null"] }
                                   },
                                   "required": ["authorized", "courier_id"],
                                   "additionalProperties": false
                                 }
                                 """,
            ResponseBodyJson = """{"authorized":false,"courier_id":null}"""
        });
        await database.SaveChangesAsync();
    }
}