using MCPLearning.Services;
using Microsoft.AspNetCore.Mvc;

namespace MCPLearning.Controllers;

[ApiController]
[Route("api/customers")]
public class CustomerController : ControllerBase
{
    private readonly CustomerRepository _repository;

    public CustomerController(
        CustomerRepository repository)
    {
        _repository = repository;
    }

    [HttpGet("{name}")]
    public async Task<IActionResult>
        GetCustomer(string name)
    {
        var customer =
            await _repository
                .GetCustomerByNameAsync(name);

        if (customer == null)
        {
            return NotFound(
                "Customer not found.");
        }

        return Ok(customer);
    }

    [HttpGet("premium")]
    public async Task<IActionResult>
        GetPremiumCustomers()
    {
        var customers =
            await _repository
                .GetPremiumCustomersAsync();

        return Ok(customers);
    }

    [HttpGet("{name}/loan")]
    public async Task<IActionResult>
        GetLoanLimit(string name)
    {
        var loanLimit =
            await _repository
                .GetLoanLimitAsync(name);

        if (loanLimit == null)
        {
            return NotFound(
                "Customer not found.");
        }

        return Ok(
            new
            {
                Customer = name,
                LoanLimit = loanLimit
            });
    }
}