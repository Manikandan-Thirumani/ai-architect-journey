namespace AiAgentApi.Models;

public class ApprovalDecision
{
    public bool RequiresApproval { get; set; }

    public string Reason { get; set; } = "";
}