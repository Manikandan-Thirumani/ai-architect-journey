namespace AiAgentApi.Models;

public class GroundingDecision
{
    public bool IsGrounded { get; set; }

    public string Reason { get; set; } = "";
}