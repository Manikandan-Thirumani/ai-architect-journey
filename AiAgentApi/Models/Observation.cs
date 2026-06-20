namespace AiAgentApi.Models;

public class Observation
{
    public int StepNumber { get; set; }

    public string ToolName { get; set; } = "";

    public string Result { get; set; } = "";
}