namespace AiAgentApi.Models;

public class CopilotResponse
{
    public string Question { get; set; } = "";

    public string FinalAnswer { get; set; } = "";

    public bool HumanApprovalRequired { get; set; }

    public string ApprovalReason { get; set; } = "";

    public List<string> AgentsUsed { get; set; } = [];

    public string RoutingReason { get; set; } = "";

    public long ExecutionTimeMs { get; set; }
}