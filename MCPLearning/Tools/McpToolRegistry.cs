using MCPLearning.Tools;

namespace MCPLearning.Services;

public class McpToolRegistry
{
    private readonly IEnumerable<IMcpTool> _tools;

    public McpToolRegistry(
        IEnumerable<IMcpTool> tools)
    {
        _tools = tools;
    }

    public IEnumerable<IMcpTool> GetAll()
    {
        return _tools;
    }

    public IMcpTool? Get(
        string name)
    {
        return _tools.FirstOrDefault(
            x => x.Name.Equals(
                name,
                StringComparison.OrdinalIgnoreCase));
    }
}