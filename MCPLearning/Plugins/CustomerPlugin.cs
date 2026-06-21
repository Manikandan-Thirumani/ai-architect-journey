using MCPLearning.Services;
using Microsoft.SemanticKernel;
using System.ComponentModel;

namespace MCPLearning.Plugins;

public class CustomerPlugin
{
    private readonly CustomerRepository _repository;

    public CustomerPlugin(
        CustomerRepository repository)
    {
        _repository = repository;
    }

    [KernelFunction]
    [Description("Get customer details by customer name.")]
    public async Task<string> GetCustomer(
        string customerName)
    {
        var customer =
            await _repository
                .GetCustomerByNameAsync(
                    customerName);

        if (customer == null)
        {
            return "Customer not found.";
        }

        return
$"""
Customer Name: {customer.CustomerName}
Customer Type: {customer.CustomerType}
Insurance Amount: {customer.InsuranceAmount}
Loan Limit: {customer.LoanLimit}
""";
    }
}