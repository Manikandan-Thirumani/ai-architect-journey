using Microsoft.SemanticKernel;

namespace AiAgentApi.Agents;

public class ReviewerAgent
{
    private readonly Kernel _kernel;

    public ReviewerAgent(
        Kernel kernel)
    {
        _kernel = kernel;
    }

    public async Task<string> ReviewAsync(
        List<Models.AgentResult> results,
        string originalQuestion)
    {
        var observations =
            string.Join(
                Environment.NewLine + Environment.NewLine,
                results.Select(x =>
                    $"Agent: {x.AgentName}" +
                    Environment.NewLine +
                    $"Response: {x.Response}"));

        var prompt = $$"""
You are an enterprise reviewer agent.

Original Question:
{{originalQuestion}}

Agent Findings:
{{observations}}

Instructions:
- Review all findings.
- Identify agreements or conflicts.
- Produce the best final answer.
- Prefer safety when uncertainty exists.
- Do not mention internal agents.
- Respond directly to the user.

Final Answer:
""";

        var response =
            await _kernel.InvokePromptAsync(prompt);

        return response.ToString();
    }
}