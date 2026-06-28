using EnterpriseSqlCopilot.Models;
using Microsoft.SemanticKernel;
using System.Text.Json;

namespace EnterpriseSqlCopilot.Services;

public class McpPlannerService
{
    private readonly Kernel _kernel;

    public McpPlannerService(
        Kernel kernel)
    {
        _kernel = kernel;
    }

    public async Task<List<AgentToolCall>>
        CreatePlanAsync(
            object tools,
            string question)
    {
        var prompt =
$$"""
You are an MCP agent planner.

Available MCP tools:

{{JsonSerializer.Serialize(
    tools,
    new JsonSerializerOptions
    {
        WriteIndented = true
    })}}

Question:
{{question}}

Rules:
- Return ONLY JSON.
- Never return markdown.
- Never return explanation.
- Return a JSON array.
- Always call get_schema first.
- Then validate_sql.
- Then execute_sql.
- Then explain_sql.

Example:

[
    {
        "tool":"get_schema",
        "arguments":{}
    },
    {
        "tool":"validate_sql",
        "arguments":
        {
            "sql":"GENERATED_SQL"
        }
    },
    {
        "tool":"execute_sql",
        "arguments":
        {
            "sql":"GENERATED_SQL"
        }
    },
    {
        "tool":"explain_sql",
        "arguments":
        {
            "sql":"GENERATED_SQL"
        }
    }
]
""";

        var response =
            await _kernel
                .InvokePromptAsync(
                    prompt);

        var json =
            response
                .ToString()
                .Replace(
                    "```json",
                    "")
                .Replace(
                    "```",
                    "")
                .Trim();

        Console.WriteLine(
            "Raw planner response:");

        Console.WriteLine(
            json);

        try
        {
            var plan =
                JsonSerializer
                    .Deserialize<
                        List<AgentToolCall>>(
                            json);

            if (plan != null &&
                plan.Count > 0)
            {
                Console.WriteLine(
                    "Planner succeeded.");

                return plan;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(
                "Planner parsing failed:");

            Console.WriteLine(
                ex.Message);
        }

        /*
         * Fallback plan.
         * Local LLMs frequently fail
         * to emit valid JSON.
         */
        Console.WriteLine(
            "Planner failed. Using fallback plan.");

        return
        [
            new AgentToolCall
            {
                Tool = "get_schema",
                Arguments =
                    new Dictionary<string, object>()
            },

            new AgentToolCall
            {
                Tool = "validate_sql",
                Arguments =
                    new Dictionary<string, object>()
            },

            new AgentToolCall
            {
                Tool = "execute_sql",
                Arguments =
                    new Dictionary<string, object>()
            },

            new AgentToolCall
            {
                Tool = "explain_sql",
                Arguments =
                    new Dictionary<string, object>()
            }
        ];
    }
}