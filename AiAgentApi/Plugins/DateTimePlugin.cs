using Microsoft.SemanticKernel;

namespace AiAgentApi.Plugins;

public class DateTimePlugin
{
    [KernelFunction]
    public string GetCurrentDate()
    {
        return DateTime.Now.ToString(
            "dddd, dd MMMM yyyy");
    }

    [KernelFunction]
    public string GetCurrentTime()
    {
        return DateTime.Now.ToString(
            "hh:mm:ss tt");
    }
}