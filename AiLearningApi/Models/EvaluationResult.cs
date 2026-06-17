namespace AiLearningApi.Models;

public class EvaluationResult
{
    public string Question { get; set; } = string.Empty;

    public double Recall { get; set; }

    public double Precision { get; set; }

    public double Mrr { get; set; }

    public List<string> RetrievedDocuments
    { get; set; } = new();
}