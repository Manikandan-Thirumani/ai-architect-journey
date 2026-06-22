namespace MCPLearning.Services;

public class McpAuthorizationService
{
    private readonly Dictionary<string, List<string>>
        _permissions =
            new()
            {
                {
                    "Customer",
                    new()
                    {
                        "GetInsuranceAmount"
                    }
                },

                {
                    "Manager",
                    new()
                    {
                        "GetInsuranceAmount",
                        "GetLoanLimit"
                    }
                },

                {
                    "Admin",
                    new()
                    {
                        "GetInsuranceAmount",
                        "GetLoanLimit",
                        "GetCustomerByName"
                    }
                }
            };

    public bool CanExecute(
        string role,
        string toolName)
    {
        return _permissions.TryGetValue(
                   role,
                   out var tools)
               &&
               tools.Contains(toolName);
    }
}