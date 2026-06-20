using AiAgentApi.Models;
using AiAgentApi.Services;
using Microsoft.SemanticKernel;

namespace AiAgentApi.Agents;

public class ReviewerAgent
{
    private readonly Kernel _kernel;
    private readonly IConversationMemoryService _memory;

    public ReviewerAgent(
        Kernel kernel,
        IConversationMemoryService memory)
    {
        _kernel = kernel;
        _memory = memory;
    }

    public async Task<string> ReviewAsync(string userid,
        List<AgentResult> results,
        string originalQuestion)
    {
        /*
         * Week 11 Day 3
         * Embedding-based memory retrieval
         */

        var relevantMemories =
            await _memory.SearchRelevantByEmbedding(userid,
                originalQuestion,
                3);

        Console.WriteLine("Relevant Memories:");

        foreach (var item in relevantMemories)
        {
            Console.WriteLine(
                $"{item.Role}: {item.Content}");
        }

        var history =
            string.Join(
                Environment.NewLine,
                relevantMemories.Select(x =>
                    $"{x.Role}: {x.Content}"));

        /*
         * Do NOT expose agent names.
         */

        var observations =
            string.Join(
                Environment.NewLine + Environment.NewLine,
                results.Select(x => x.Response));

        var prompt = $$"""
You are an enterprise reviewer agent.

Recent Conversation:
{{history}}

Original Question:
{{originalQuestion}}

Evidence:
{{observations}}

Instructions:
- Use the recent conversation to understand context.
- Resolve references such as:
  "their", "they", "those customers", "that policy".
- Use ONLY the evidence provided.
- Include only information relevant to the user's question.
- Ignore unrelated evidence.
- Do not add assumptions.
- Do not invent details.
- Internal system components are confidential.
- Never mention agent names.
- Never mention routing, tools, plugins, or reviewers.
- Never mention conversation history.
- Respond directly to the user.
- If the answer cannot be determined from the evidence,
  clearly state that more information is required.
- Keep the answer concise.

Final Answer:
""";

        Console.WriteLine("Reviewer Prompt:");
        Console.WriteLine(prompt);

        var response =
            await _kernel.InvokePromptAsync(prompt);

        var finalAnswer =
            response.ToString();

        Console.WriteLine("Reviewer Response:");
        Console.WriteLine(finalAnswer);

        return finalAnswer;
    }
}