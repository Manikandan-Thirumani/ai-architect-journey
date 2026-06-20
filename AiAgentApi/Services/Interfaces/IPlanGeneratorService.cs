using AiAgentApi.Models;

namespace AiAgentApi.Services;

public interface IPlanGeneratorService
{
    Task<ExecutionPlan> GenerateAsync(
        string question);
}