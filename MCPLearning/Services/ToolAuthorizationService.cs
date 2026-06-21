namespace MCPLearning.Services;

public class ToolAuthorizationService
{
    private readonly Dictionary<string, List<string>>
        _permissions =
            new()
            {
                {
                    "Customer",
                    new List<string>
                    {
                        "Customer",
                        "Date"
                    }
                },

                {
                    "Manager",
                    new List<string>
                    {
                        "Customer",
                        "Date",
                        "Currency"
                    }
                },

                {
                    "Admin",
                    new List<string>
                    {
                        "Customer",
                        "Date",
                        "Currency"
                    }
                }
            };

    public bool CanUseTool(
        string role,
        string toolName)
    {
        if (!_permissions.ContainsKey(role))
        {
            return false;
        }

        return _permissions[role]
            .Contains(toolName);
    }
}