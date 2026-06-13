namespace AiLearningApi.Models;

public class SourceCitation
{
    public string DocumentName
    {
        get;
        set;
    } = string.Empty;

    public string PolicyName
    {
        get;
        set;
    } = string.Empty;
    public double Score { get; set; }
    public string SourceDocument { get; set; } = string.Empty;
}