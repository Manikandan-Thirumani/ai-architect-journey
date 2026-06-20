
using AiAgentApi.Models;
using AiAgentApi.Services;
using Microsoft.SemanticKernel;
using System.Diagnostics;
using System.Text.Json;

namespace AiAgentApi.Agents;

public class CoordinatorAgent
{
    private readonly RetrievalAgent _retrievalAgent;
    private readonly DateAgent _dateAgent;
    private readonly ReviewerAgent _reviewerAgent;
    private readonly HumanApprovalAgent _humanApprovalAgent;
    private readonly GroundingValidatorAgent _groundingValidator;
    private readonly Kernel _kernel;
    private readonly IConversationMemoryService _memory;

    public CoordinatorAgent(
        RetrievalAgent retrievalAgent,
        DateAgent dateAgent,
        ReviewerAgent reviewerAgent,
        Kernel kernel,
        HumanApprovalAgent humanApprovalAgent,
        IConversationMemoryService memory,
        GroundingValidatorAgent groundingValidator)
    {
        _retrievalAgent = retrievalAgent;
        _dateAgent = dateAgent;
        _reviewerAgent = reviewerAgent;
        _kernel = kernel;
        _humanApprovalAgent = humanApprovalAgent;
        _memory = memory;
        _groundingValidator = groundingValidator;
    }

    public async Task<CopilotResponse> ExecuteAsync(
        string userId,
        string question)
    {
        /*
         * Save user question into memory.
         */
        await _memory.AddAsync(
            userId,
            "User",
            question);

        var stopwatch =
            Stopwatch.StartNew();

        var routing =
            await RouteAgents(question);

        Console.WriteLine(
            $"Selected Agents: {string.Join(", ", routing.Agents)}");

        Console.WriteLine(
            $"Routing Reason: {routing.Reason}");

        var tasks =
            new List<Task<AgentResult>>();

        foreach (var agent in routing.Agents)
        {
            switch (agent)
            {
                case "RetrievalAgent":

                    Console.WriteLine(
                        "Scheduling RetrievalAgent...");

                    tasks.Add(
                        ExecuteAgent(
                            "RetrievalAgent",
                            _retrievalAgent.ExecuteAsync(question)));

                    break;

                case "DateAgent":

                    Console.WriteLine(
                        "Scheduling DateAgent...");

                    tasks.Add(
                        ExecuteAgent(
                            "DateAgent",
                            _dateAgent.ExecuteAsync(question)));

                    break;

                default:

                    Console.WriteLine(
                        $"Unknown agent: {agent}");

                    break;
            }
        }

        if (tasks.Count == 0)
        {
            stopwatch.Stop();

            return new CopilotResponse
            {
                Question = question,
                FinalAnswer = "No suitable agent found.",
                HumanApprovalRequired = false,
                ApprovalReason = "",
                AgentsUsed = routing.Agents,
                RoutingReason = routing.Reason,
                ExecutionTimeMs = stopwatch.ElapsedMilliseconds
            };
        }

        Console.WriteLine(
            $"Executing {tasks.Count} agents in parallel...");

        var results =
            await Task.WhenAll(tasks);

        /*
         * Week 10 Day 4
         * Conflict Resolution
         */
        var resolvedResults =
            ResolveConflicts(results.ToList());

        Console.WriteLine(
            "Conflict Resolution Completed.");

        /*
         * Week 11 Day 7
         * Grounding Validation
         */
        Console.WriteLine(
            "Validating evidence grounding...");

        var grounding =
            await _groundingValidator.ValidateAsync(
                question,
                resolvedResults);

        Console.WriteLine(
            $"Grounded: {grounding.IsGrounded}");

        Console.WriteLine(
            $"Grounding Reason: {grounding.Reason}");

        if (!grounding.IsGrounded)
        {
            stopwatch.Stop();

            var refusalMessage =
                "I could not find information related to this question in the available knowledge base.";

            /*
             * Save refusal response into memory.
             */
            await _memory.AddAsync(
                userId,
                "Assistant",
                refusalMessage);

            return new CopilotResponse
            {
                Question = question,

                FinalAnswer = refusalMessage,

                HumanApprovalRequired = false,

                ApprovalReason = "",

                AgentsUsed = routing.Agents,

                RoutingReason = routing.Reason,

                ExecutionTimeMs =
                    stopwatch.ElapsedMilliseconds
            };
        }

        /*
         * Week 10 Day 5
         * Reviewer Agent
         */
        Console.WriteLine(
            "Sending findings to ReviewerAgent...");

        var reviewedResponse =
            await _reviewerAgent.ReviewAsync(
                userId,
                resolvedResults,
                question);

        /*
         * Week 10 Day 6
         * Human Approval
         */
        var approval =
            _humanApprovalAgent.Evaluate(
                question,
                reviewedResponse);

        Console.WriteLine(
            $"Approval Required: {approval.RequiresApproval}");

        stopwatch.Stop();

        /*
         * Save assistant response into memory.
         */
        await _memory.AddAsync(
            userId,
            "Assistant",
            reviewedResponse);

        /*
         * Enterprise Copilot Response
         */
        return new CopilotResponse
        {
            Question = question,

            FinalAnswer = reviewedResponse,

            HumanApprovalRequired =
                approval.RequiresApproval,

            ApprovalReason =
                approval.Reason,

            AgentsUsed =
                routing.Agents,

            RoutingReason =
                routing.Reason,

            ExecutionTimeMs =
                stopwatch.ElapsedMilliseconds
        };
    }

    private async Task<RoutingDecision>
        RouteAgents(string question)
    {
        var prompt = $$"""
You are an enterprise agent router.

Available agents:

1. RetrievalAgent
Use for:
- benefits
- insurance
- loans
- cashback
- KYC
- banking policies

2. DateAgent
Use for:
- today's date
- current date
- today's day

Return ONLY valid JSON.

Example:

{
    "agents": [
        "RetrievalAgent",
        "DateAgent"
    ],
    "reason": "The question requires knowledge lookup and date."
}

Rules:
- Return JSON only.
- Do not use markdown.
- Do not add explanations.
- Agent names MUST be:
  "RetrievalAgent"
  "DateAgent"

Question:
{{question}}
""";

        try
        {
            var response =
                await _kernel.InvokePromptAsync(prompt);

            var json =
                response.ToString();

            Console.WriteLine("Router Output:");
            Console.WriteLine(json);

            json = json.Replace("```json", "");
            json = json.Replace("```", "");

            var start =
                json.IndexOf('{');

            var end =
                json.LastIndexOf('}');

            if (start >= 0 &&
                end > start)
            {
                json = json.Substring(
                    start,
                    end - start + 1);
            }

            Console.WriteLine(
                "Cleaned Router JSON:");

            Console.WriteLine(json);

            var decision =
                JsonSerializer.Deserialize<RoutingDecision>(
                    json,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

            if (decision != null)
            {
                return decision;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(
                $"Router Error: {ex.Message}");
        }

        /*
         * Fallback Routing
         */
        Console.WriteLine(
            "Router failed. Using fallback routing.");

        var fallback =
            new RoutingDecision();

        var lower =
            question.ToLowerInvariant();

        if (lower.Contains("benefit") ||
            lower.Contains("insurance") ||
            lower.Contains("loan") ||
            lower.Contains("cashback") ||
            lower.Contains("kyc"))
        {
            fallback.Agents.Add(
                "RetrievalAgent");
        }

        if (lower.Contains("date") ||
            lower.Contains("today"))
        {
            fallback.Agents.Add(
                "DateAgent");
        }

        fallback.Reason =
            "Fallback routing";

        return fallback;
    }

    private async Task<AgentResult>
        ExecuteAgent(
            string agentName,
            Task<string> task)
    {
        Console.WriteLine(
            $"{agentName} executing...");

        return new AgentResult
        {
            AgentName = agentName,
            Response = await task
        };
    }

    private List<AgentResult>
        ResolveConflicts(
            List<AgentResult> results)
    {
        Console.WriteLine(
            "Conflict Resolution Started");

        var uniqueResponses =
            results
                .GroupBy(x => x.Response)
                .Select(g => g.First())
                .ToList();

        if (uniqueResponses.Count != results.Count)
        {
            Console.WriteLine(
                "Duplicate responses detected.");
        }

        return uniqueResponses;
    }
}

