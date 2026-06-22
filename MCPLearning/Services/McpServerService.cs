using MCPLearning.Services;

namespace MCPLearning.MCP;

public class McpServerService
{
    private readonly CustomerRepository _repository;

    private readonly McpAuthorizationService
        _authorization;
    private readonly AuditService _audit;
    private readonly McpRateLimitService
    _rateLimiter;

    public McpServerService(
        CustomerRepository repository,
        McpAuthorizationService authorization,
        AuditService audit,
        McpRateLimitService rateLimiter)
    {
        _repository = repository;
        _authorization = authorization;
        _audit = audit;
        _rateLimiter = rateLimiter;
    }

    public List<McpToolDefinition> GetTools()
    {
        return
        [
            new()
            {
                Name = "GetCustomerByName",

                Description =
                    "Returns customer details by customer name.",

                InputSchema = new
                {
                    type = "object",

                    properties = new
                    {
                        customerName = new
                        {
                            type = "string"
                        }
                    },

                    required =
                        new[] { "customerName" }
                }
            },

            new()
            {
                Name = "GetLoanLimit",

                Description =
                    "Returns loan limit by customer name.",

                InputSchema = new
                {
                    type = "object",

                    properties = new
                    {
                        customerName = new
                        {
                            type = "string"
                        }
                    },

                    required =
                        new[] { "customerName" }
                }
            },

            new()
            {
                Name = "GetInsuranceAmount",

                Description =
                    "Returns insurance amount by customer name.",

                InputSchema = new
                {
                    type = "object",

                    properties = new
                    {
                        customerName = new
                        {
                            type = "string"
                        }
                    },

                    required =
                        new[] { "customerName" }
                }
            }
        ];
    }

    public async Task<object> CallToolAsync(
        string role,
        string toolName,
        Dictionary<string, object> arguments)
    {
        Console.WriteLine(
            $"Role={role}, Tool={toolName}");

        /*
         * Week 13 Day 4
         * Role-based authorization.
         */
        if (!_authorization.CanExecute(
                role,
                toolName))
        {
            _audit.Log(
    role,
    toolName,
    arguments,
    "Denied",
    "Access denied.");

            throw new UnauthorizedAccessException(
                "Access denied.");
        }

        switch (toolName)
        {
            case "GetCustomerByName":
                {
                    if (!arguments.ContainsKey(
                            "customerName"))
                    {
                        throw new ArgumentException(
                            "customerName is required.");
                    }

                    var customerName =
                        arguments["customerName"]
                            ?.ToString();

                    var customer =
                        await _repository
                            .GetCustomerByNameAsync(
                                customerName!);

                    if (customer == null)
                    {
                        return new
                        {
                            Message =
                                "Customer not found."
                        };
                    }

                    return customer;
                }

            case "GetLoanLimit":
                {
                    if (!arguments.ContainsKey(
                            "customerName"))
                    {
                        throw new ArgumentException(
                            "customerName is required.");
                    }

                    var customerName =
                        arguments["customerName"]
                            ?.ToString();

                    var limit =
                        await _repository
                            .GetLoanLimitAsync(
                                customerName!);

                    return new
                    {
                        CustomerName =
                            customerName,

                        LoanLimit =
                            limit
                    };
                }

            case "GetInsuranceAmount":
                {
                    if (!arguments.ContainsKey(
                            "customerName"))
                    {
                        throw new ArgumentException(
                            "customerName is required.");
                    }

                    var customerName =
                        arguments["customerName"]
                            ?.ToString();

                    var customer =
                        await _repository
                            .GetCustomerByNameAsync(
                                customerName!);

                    if (customer == null)
                    {
                        return new
                        {
                            Message =
                                "Customer not found."
                        };
                    }

                    return new
                    {
                        CustomerName =
                            customer.CustomerName,

                        InsuranceAmount =
                            customer.InsuranceAmount
                    };
                }

            default:

                throw new Exception(
                    $"Unknown tool: {toolName}");
        }
    }
}