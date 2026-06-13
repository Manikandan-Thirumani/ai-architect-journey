using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.Ollama;

namespace AiLearningApi.Services;

public class QueryRewriteService
{
    private readonly Kernel _kernel;

    public QueryRewriteService()
    {
        var builder =
            Kernel.CreateBuilder();

        builder.AddOllamaChatCompletion(
            modelId: "phi3",
            endpoint:
                new Uri("http://localhost:11434"));

        _kernel = builder.Build();
    }

    public async Task<string> Rewrite(
        string conversationHistory,
        string currentQuestion)
    {
        var prompt = $"""
You are a banking conversation assistant.

Conversation History:
{conversationHistory}

Current Question:
{currentQuestion}

TASK

Rewrite the current question ONLY if it depends on
previous conversation context.

If the question is already complete,
return it unchanged.

RULES

1. Preserve banking product context.
   Examples:
   - Loan
   - Credit Card
   - Insurance
   - Deposit
   - KYC

2. Resolve references such as:
   - it
   - they
   - them
   - those
   - what about
   - regular customers
   - premium customers

3. Use ONLY information found in conversation history.

4. Do NOT invent:
   - policy names
   - chapter names
   - chapter numbers
   - customer types
   - product names

5. Return ONE standalone question.

6. Return ONLY the rewritten question.

EXAMPLES

Conversation:
User: What is the premium loan amount?

Current:
What about regular customers?

Output:
What is the loan amount for regular customers?

Conversation:
User: What benefits do premium credit card customers get?

Current:
What benefits do premium customers receive?

Output:
What benefits do premium credit card customers receive?

Conversation:
User: What is the interest rate for fixed deposits?

Current:
What about senior citizens?

Output:
What is the interest rate for senior citizens fixed deposits?

FINAL QUESTION:
""";

        var result =
            await _kernel.InvokePromptAsync(
                prompt);

        return result.ToString().Trim();
    }
}