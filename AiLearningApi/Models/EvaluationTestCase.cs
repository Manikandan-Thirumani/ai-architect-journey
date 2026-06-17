namespace AiLearningApi.Models;

public class EvaluationTestCase
{
    public string Question { get; set; } = string.Empty;

    public List<string> ExpectedDocuments
    { get; set; } = new();
}