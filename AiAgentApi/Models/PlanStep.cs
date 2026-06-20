namespace AiAgentApi.Models;

public class PlanStep
{
    public int StepNumber { get; set; }

    public string ToolName { get; set; } = "";

    public string Input { get; set; } = "";
}