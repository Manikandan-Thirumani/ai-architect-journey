namespace AiAgentApi.Models;

public class ExecutionPlan
{
    public List<PlanStep> Steps { get; set; }
        = new();
}