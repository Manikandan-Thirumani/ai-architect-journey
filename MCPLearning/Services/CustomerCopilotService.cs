using MCPLearning.Plugins;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.Ollama;

namespace MCPLearning.Services;

public class CustomerCopilotService
{
    private readonly Kernel _kernel;

    private readonly CustomerPlugin _plugin;

    private readonly DatePlugin _datePlugin;

    private readonly CurrencyPlugin _currencyPlugin;

    private readonly ToolAuthorizationService _authorization;

    public CustomerCopilotService(
        Kernel kernel,
        CustomerPlugin plugin,
        DatePlugin datePlugin,
        CurrencyPlugin currencyPlugin,
        ToolAuthorizationService authorization)
    {
        _kernel = kernel;
        _plugin = plugin;
        _datePlugin = datePlugin;
        _currencyPlugin = currencyPlugin;
        _authorization = authorization;
    }

    public async Task<string> AskAsync(
        string question,
        string role)
    {
        Console.WriteLine($"Role: {role}");
        Console.WriteLine($"Question: {question}");

        /*
         * Enterprise Guardrail:
         * Enforce authorization BEFORE LLM invocation.
         */

        var lowerQuestion =
            question.ToLowerInvariant();

        if ((lowerQuestion.Contains("exchange") ||
             lowerQuestion.Contains("usd") ||
             lowerQuestion.Contains("inr") ||
             lowerQuestion.Contains("currency"))
            &&
            !_authorization.CanUseTool(
                role,
                "Currency"))
        {
            return
                "You are not authorized to access exchange-rate information.";
        }

        if ((lowerQuestion.Contains("insurance") ||
             lowerQuestion.Contains("loan") ||
             lowerQuestion.Contains("premium") ||
             lowerQuestion.Contains("customer"))
            &&
            !_authorization.CanUseTool(
                role,
                "Customer"))
        {
            return
                "You are not authorized to access customer information.";
        }

        /*
         * Clear previously registered plugins.
         */
        _kernel.Plugins.Clear();

        /*
         * Register only authorized plugins.
         */

        var availableTools =
            new List<string>();

        if (_authorization.CanUseTool(
                role,
                "Customer"))
        {
            _kernel.Plugins.AddFromObject(
                _plugin,
                "Customer");

            availableTools.Add(
                "- Customer: Insurance amount, loan limits, customer type, premium customers");

            Console.WriteLine(
                $"Customer plugin enabled for role {role}");
        }

        if (_authorization.CanUseTool(
                role,
                "Date"))
        {
            _kernel.Plugins.AddFromObject(
                _datePlugin,
                "Date");

            availableTools.Add(
                "- Date: Today's date, current day, current time");

            Console.WriteLine(
                $"Date plugin enabled for role {role}");
        }

        if (_authorization.CanUseTool(
                role,
                "Currency"))
        {
            _kernel.Plugins.AddFromObject(
                _currencyPlugin,
                "Currency");

            availableTools.Add(
                "- Currency: USD to INR exchange rates");

            Console.WriteLine(
                $"Currency plugin enabled for role {role}");
        }

        /*
         * Enable automatic tool calling.
         */
        var settings =
            new OllamaPromptExecutionSettings
            {
                FunctionChoiceBehavior =
                    FunctionChoiceBehavior.Auto()
            };

        /*
         * Build prompt dynamically.
         */
        var prompt =
$"""
You are an enterprise banking copilot.

Current User Role:
{role}

Available Tools:
{string.Join(Environment.NewLine, availableTools)}

Rules:
- Use ONLY the available tools.
- Do not answer using prior knowledge.
- Do not guess.
- Do not invent information.
- If information cannot be found,
  clearly state that it is unavailable.
- Combine outputs from multiple tools when required.
- Respond directly to the user.

Question:
{question}
""";

        Console.WriteLine("Prompt:");
        Console.WriteLine(prompt);

        var response =
            await _kernel.InvokePromptAsync(
                prompt,
                new(settings));

        var answer =
            response.ToString();

        Console.WriteLine("Final Response:");
        Console.WriteLine(answer);

        return answer;
    }
}