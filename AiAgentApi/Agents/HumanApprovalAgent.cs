using AiAgentApi.Models;

namespace AiAgentApi.Agents;

public class HumanApprovalAgent
{
    public ApprovalDecision Evaluate(
        string question,
        string recommendation)
    {
        var lower =
            (question + " " + recommendation)
            .ToLowerInvariant();

        var requiresApproval =
            lower.Contains("reject") ||
            lower.Contains("approve loan") ||
            lower.Contains("fraud") ||
            lower.Contains("freeze account") ||
            lower.Contains("insurance claim");

        return new ApprovalDecision
        {
            RequiresApproval = requiresApproval,

            Reason = requiresApproval
                ? "High-risk decision detected."
                : "No approval required."
        };
    }
}