namespace AiLearningApi.Models;

public class RagEvaluationResult
{
    public string Question { get; set; } = string.Empty;

    public string ExpectedAnswer { get; set; } = string.Empty;

    public string ActualAnswer { get; set; } = string.Empty;

    public double RetrievalScore { get; set; }

    public double AnswerScore { get; set; }

    public double FinalScore { get; set; }
}