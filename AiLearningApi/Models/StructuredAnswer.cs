namespace AiLearningApi.Models;

public class StructuredAnswer
{
    public string Answer { get; set; } = string.Empty;

    public double Confidence { get; set; }

    public List<string> Sources { get; set; } = new();

    public bool IsGrounded { get; set; }
}