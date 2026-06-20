namespace AiAgentApi.Models;

public class ObservationValidationResult
{
    public bool IsRelevant { get; set; }

    public string Reason { get; set; } = "";
}