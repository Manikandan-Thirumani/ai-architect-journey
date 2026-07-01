using Microsoft.SemanticKernel;
using ShoppingAgentClient.MCP;
using ShoppingAgentClient.Models;
using System.Text.Json;

namespace ShoppingAgentClient.Services;

public class ShoppingAgent
{
    private readonly ShoppingMcpClient
        _mcp;

    private readonly Kernel
        _kernel;

    public ShoppingAgent(
        ShoppingMcpClient mcp,
        Kernel kernel)
    {
        _mcp = mcp;
        _kernel = kernel;
    }

    public async Task<object>
        AskAsync(
            string question)
    {
        /*
         * STEP 1
         * Discover tools
         */
        Console.WriteLine(
            "Discovering MCP tools");

        var tools =
            await _mcp
                .ListToolsAsync();

        /*
         * STEP 2
         * Create memory
         */
        var memory =
            new AgentMemory
            {
                Question =
                    question
            };

        /*
         * STEP 3
         * ReAct Loop
         */
        while (true)
        {
            var prompt = $$"""
You are an AI shopping agent.

User request:

{{question}}

Examples:

{
    "finish":false,
    "tool":"search_products",
    "arguments":
    {
        "keyword":"iphone"
    }
}

{
    "finish":true,
    "answer":"Order placed successfully"
}
""";

            Console.WriteLine(
                "================================");

            Console.WriteLine(
                "PROMPT:");

            Console.WriteLine(
                prompt);

            Console.WriteLine(
                "================================");

            /*
             * STEP 4
             * Ask LLM
             */
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
                "LLM RESPONSE:");

            Console.WriteLine(
                json);

            /*
             * STEP 5
             * Parse decision
             */
            AgentToolDecision?
                decision;

            try
            {
                decision =
                    JsonSerializer
                        .Deserialize<
                            AgentToolDecision>(
                                json);
            }
            catch
            {
                return new
                {
                    Question =
                        question,

                    Memory =
                        memory,

                    PlannerResponse =
                        json,

                    Error =
                        "Planner failed"
                };
            }

            if (decision == null)
            {
                return new
                {
                    Question =
                        question,

                    Memory =
                        memory,

                    Error =
                        "Planner returned null"
                };
            }

            /*
             * STEP 6
             * Finished?
             */
            if (decision
                .Finish)
            {
                return new
                {
                    Question =
                        question,

                    Memory =
                        memory,

                    Answer =
                        decision
                            .Answer
                };
            }

            /*
             * STEP 7
             * Execute MCP tool
             */
            object result;

            try
            {
                result =
                    await _mcp
                        .CallToolAsync(
                            decision.Tool,
                            decision.Arguments);
            }
            catch (Exception ex)
            {
                result =
                    ex.Message;
            }

            /*
             * STEP 8
             * Store observation
             */
            memory
                .History
                .Add(
                    new AgentHistory
                    {
                        Step =
                            memory
                                .History
                                .Count + 1,

                        Tool =
                            decision
                                .Tool,

                        Result =
                            result
                    });

            Console.WriteLine(
                "================================");

            Console.WriteLine(
                "OBSERVATION:");

            Console.WriteLine(
                JsonSerializer
                    .Serialize(
                        result,
                        new JsonSerializerOptions
                        {
                            WriteIndented =
                                true
                        }));

            Console.WriteLine(
                "================================");
        }
    }
}