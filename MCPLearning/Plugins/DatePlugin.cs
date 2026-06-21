using Microsoft.SemanticKernel;

namespace MCPLearning.Plugins;

public class DatePlugin
{
    [KernelFunction]
    public string GetToday()
    {
        return DateTime.Now.ToString("dddd, dd MMMM yyyy");
    }

    [KernelFunction]
    public string GetCurrentTime()
    {
        return DateTime.Now.ToString("HH:mm:ss");
    }
}