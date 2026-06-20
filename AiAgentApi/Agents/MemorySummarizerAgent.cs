using AiAgentApi.Services;
using Microsoft.SemanticKernel;

namespace AiAgentApi.Agents;

public class MemorySummarizerAgent
{
    private readonly Kernel _kernel;
    private readonly IConversationMemoryService _memory;

    public MemorySummarizerAgent(
        Kernel kernel,
        IConversationMemoryService memory)
    {
        _kernel = kernel;
        _memory = memory;
    }

    public async Task SummarizeAsync(
        string userId)
    {
        var history =
            _memory.GetHistory(userId);

        if (history.Count < 10)
        {
            Console.WriteLine(
                "Not enough memories to summarize.");

            return;
        }

        var conversations =
            string.Join(
                Environment.NewLine,
                history.Select(x =>
                    $"{x.Role}: {x.Content}"));

        var prompt = $$"""
You are a memory summarization agent.

Summarize the following conversation.

Extract only long-term useful facts.

Examples:
- customer type
- preferences
- important profile details
- recurring goals

Ignore:
- greetings
- temporary questions
- one-time requests

Conversation:
{{conversations}}

Summary:
""";

        var response =
            await _kernel.InvokePromptAsync(prompt);

        var summary =
            response.ToString();

        Console.WriteLine("Summary:");
        Console.WriteLine(summary);

        await _memory.AddAsync(
            userId,
            "Summary",
            summary);

        Console.WriteLine(
            "Summary memory saved.");
    }
}