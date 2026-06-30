using Microsoft.SemanticKernel;
using ShoppingAgentClient.MCP;
using ShoppingAgentClient.Models;

namespace ShoppingAgentClient.Services;

public class ShoppingAgent
{
    private readonly
        ShoppingMcpClient
            _mcp;

    private readonly
        Kernel
            _kernel;

    public ShoppingAgent(
        ShoppingMcpClient mcp,
        Kernel kernel)
    {
        _mcp =
            mcp;

        _kernel =
            kernel;
    }

    public async Task<object>
        AskAsync(
            string question)
    {
        /*
         * STEP 1
         * Discover tools.
         */
        Console.WriteLine(
            "Discovering MCP tools");

        await _mcp
            .ListToolsAsync();

        /*
         * STEP 2
         * Create memory.
         */
        var memory =
            new AgentMemory
            {
                Question =
                    question
            };

        /*
         * STEP 3
         * Ask LLM to select tool.
         */
        var plannerPrompt =
$$"""
You are a shopping agent.

Available tools:

search_products
get_product
add_to_cart
view_cart
remove_from_cart
place_order
get_orders
get_order
cancel_order

Question:

{{question}}

Return ONLY ONE tool name.

Examples:

buy iphone
search_products

show cart
view_cart

place order
place_order

remove product
remove_from_cart

Question:

{{question}}

Answer:
""";

        Console.WriteLine(
            "Planner Prompt:");

        Console.WriteLine(
            plannerPrompt);

        var response =
            await _kernel
                .InvokePromptAsync(
                    plannerPrompt);

        var selectedTool =
            response
                .ToString()
                .Trim()
                .Replace(
                    "\"",
                    "");

        Console.WriteLine(
            "Planner Response:");

        Console.WriteLine(
            selectedTool);

        /*
         * STEP 4
         * Create decision.
         */
        var decision =
            new ToolDecision
            {
                Tool =
                    selectedTool,

                Arguments =
                    new Dictionary<string, object>()
            };

        /*
         * STEP 5
         * Build arguments.
         */
        switch (decision.Tool)
        {
            case "search_products":
                {
                    string keyword =
                        ExtractKeyword(
                            question);

                    decision
                        .Arguments
                        .Add(
                            "keyword",
                            keyword);

                    break;
                }

            case "view_cart":
                {
                    break;
                }

            case "place_order":
                {
                    break;
                }

            case "get_orders":
                {
                    break;
                }

            default:
                {
                    return new
                    {
                        Question =
                            question,

                        PlannerResponse =
                            selectedTool,

                        Answer =
                            "Unknown tool selected."
                    };
                }
        }

        /*
         * STEP 6
         * Execute tool.
         */
        var result =
            await _mcp
                .CallToolAsync(
                    decision.Tool,
                    decision.Arguments);

        /*
         * STEP 7
         * Save history.
         */
        memory
            .History
            .Add(
                new AgentHistory
                {
                    Step = 1,

                    Tool =
                        decision.Tool,

                    Result =
                        result
                });

        /*
         * STEP 8
         * Return.
         */
        return new
        {
            Question =
                question,

            ToolDecision =
                decision,

            Memory =
                memory,

            Result =
                result,

            Answer =
                "Tool executed."
        };
    }

    private static string
        ExtractKeyword(
            string question)
    {
        question =
            question
                .ToLower();

        if (question.Contains(
                "iphone"))
        {
            return "iphone";
        }

        if (question.Contains(
                "samsung"))
        {
            return "samsung";
        }

        if (question.Contains(
                "laptop"))
        {
            return "laptop";
        }

        return question;
    }
}