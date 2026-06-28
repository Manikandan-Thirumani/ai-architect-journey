using EnterpriseSqlCopilot.MCP;
using EnterpriseSqlCopilot.Models;
using Microsoft.SemanticKernel;
using System.Text.Json;

namespace EnterpriseSqlCopilot.Services;

public class AgentLoopService
{
    private readonly Kernel _kernel;

    private readonly McpClientService _mcp;

    public AgentLoopService(
        Kernel kernel,
        McpClientService mcp)
    {
        _kernel = kernel;
        _mcp = mcp;
    }

    public async Task<object>
        RunAsync(
            string question)
    {
        var tools =
            await _mcp
                .ListToolsAsync();

        var memory =
            new AgentMemory
            {
                Question =
                    question
            };

        const int maxSteps = 10;

        for (
            int step = 1;
            step <= maxSteps;
            step++)
        {
            Console.WriteLine(
                $"Agent Step {step}");

            var decision =
                await DecideNextToolAsync(
                    tools,
                    memory);

            Console.WriteLine(
                JsonSerializer.Serialize(
                    decision,
                    new JsonSerializerOptions
                    {
                        WriteIndented = true
                    }));

            if (decision.Finish)
            {
                return new
                {
                    Question =
                        question,

                    Memory =
                        memory,

                    Answer =
                        decision.Answer
                };
            }

            var result =
                await _mcp.CallToolAsync(
                    decision.Tool,
                    decision.Arguments);

            memory.History.Add(
                new AgentStepResult
                {
                    Step = step,
                    Tool = decision.Tool,
                    Result = result
                });

            switch (decision.Tool)
            {
                case "get_schema":
                    memory.Schema =
                        result;
                    break;

                case "execute_sql":
                    memory.SqlResults =
                        result;
                    break;
            }

            if (decision.Arguments
                    .ContainsKey(
                        "sql"))
            {
                memory.GeneratedSql =
                    decision.Arguments[
                        "sql"]
                        .ToString()!;
            }

            if (ShouldStop(
                    memory.History))
            {
                return new
                {
                    Question =
                        question,

                    Memory =
                        memory,

                    Answer =
                        "Agent stopped because of loop detection."
                };
            }
        }

        return new
        {
            Question =
                question,

            Memory =
                memory,

            Answer =
                "Maximum agent steps exceeded."
        };
    }

    private bool ShouldStop(
        List<AgentStepResult>
            history)
    {
        if (history.Count < 3)
            return false;

        var recent =
            history
                .TakeLast(3)
                .Select(x => x.Tool)
                .ToList();

        return recent.All(
            x => x ==
                 recent[0]);
    }

    private async Task<
        AgentToolDecision>
        DecideNextToolAsync(
            object tools,
            AgentMemory memory)
    {
        var prompt =
$$"""
You are an autonomous MCP Agent.

Available tools:

{{JsonSerializer.Serialize(
    tools,
    new JsonSerializerOptions
    {
        WriteIndented = true
    })}}

Memory:

{{JsonSerializer.Serialize(
    memory,
    new JsonSerializerOptions
    {
        WriteIndented = true
    })}}

Rules:

Return ONLY JSON.

Examples:

{
 "finish":false,
 "tool":"get_schema",
 "arguments":{}
}

{
 "finish":false,
 "tool":"execute_sql",
 "arguments":
 {
   "sql":
   "SELECT TOP 5 * FROM Customers"
 }
}

{
 "finish":true,
 "answer":
 "The answer is..."
}
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
            "Agent Decision:");

        Console.WriteLine(
            json);

        try
        {
            return JsonSerializer
                .Deserialize
                    <AgentToolDecision>(
                        json)!;
        }
        catch
        {
            if (memory.History
                    .Count == 0)
            {
                return new AgentToolDecision
                {
                    Finish = false,
                    Tool = "get_schema",
                    Arguments =
         new Dictionary<string, object>()
                }; 
            }

            return new AgentToolDecision
            {
                Finish = true,
                Answer = "Unable to continue.",
                Tool = "",
                Arguments =
        new Dictionary<string, object>()
            };
        }
    }
}