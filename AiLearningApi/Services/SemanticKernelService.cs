using AiLearningApi.Plugins;
using Microsoft.SemanticKernel;

namespace AiLearningApi.Services;

public class SemanticKernelService
{
    public async Task<string> RunPlugin(string topic)
    {
        var builder = Kernel.CreateBuilder();

        var kernel = builder.Build();

        kernel.Plugins.AddFromObject(new ArchitectPlugin());

        var result = kernel.InvokeAsync<string>(
            "ArchitectPlugin",
            "GetBestPractice",
            new()
            {
                ["topic"] = topic
            });

        return await result;
    }
}