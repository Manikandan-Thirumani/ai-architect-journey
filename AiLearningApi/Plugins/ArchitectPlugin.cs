using Microsoft.SemanticKernel;

namespace AiLearningApi.Plugins;

public class ArchitectPlugin
{
    [KernelFunction]
    public string GetBestPractice(string topic)
    {
        return $"Best practice for {topic}: Use clean architecture and proper separation of concerns.";
    }
}