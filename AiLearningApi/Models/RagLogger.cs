using AiLearningApi.Models;

namespace AiLearningApi.Services;

public class RagLogger
{
    public RagTrace Trace { get; private set; } = new();

    public void SetQuestion(string question)
    {
        Trace.Question = question;
        AddStep($"Question received: {question}");
    }

    public void AddStep(string step)
    {
        Trace.Steps.Add($"{DateTime.UtcNow:O} - {step}");
    }

    public RagTrace GetTrace()
    {
        return Trace;
    }
}