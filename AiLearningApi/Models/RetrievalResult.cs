namespace AiLearningApi.Models;

public class RetrievalResult
{
    public string Content { get; set; } = "";

    public string DocumentName { get; set; } = "";

    public string PolicyName { get; set; } = "";
    public double Score { get; set; }
}