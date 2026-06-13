using AiLearningApi.Models;

namespace AiLearningApi.Services;

public class RagService
{
    private readonly RagOrchestrator _orchestrator;

    public RagService(RagOrchestrator orchestrator)
    {
        _orchestrator = orchestrator;
    }

    public async Task<RagResponse> Ask(string question)
    {
        return await _orchestrator.ExecuteAsync(question);
    }
}