using MCPLearning.Services;

namespace MCPLearning.Tools;

public class GetCustomerTool : IMcpTool
{
    private readonly CustomerRepository _customerService;

    public GetCustomerTool(
        CustomerRepository customerService)
    {
        _customerService = customerService;
    }

    public string Name =>
        "GetCustomer";

    public string Description =>
        "Get customer details by customer name.";

    public object InputSchema =>
        new
        {
            customerName = "string"
        };

    public async Task<object?> ExecuteAsync(
        Dictionary<string, object> arguments)
    {
        if (!arguments.TryGetValue(
                "customerName",
                out var value))
        {
            throw new Exception(
                "customerName is required.");
        }

        var customerName =
            value.ToString()!;

        return await _customerService
            .GetCustomerAsync(customerName);
    }
}