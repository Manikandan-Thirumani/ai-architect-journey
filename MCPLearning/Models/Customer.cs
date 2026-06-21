namespace MCPLearning.Models;

public class Customer
{
    public int CustomerId { get; set; }

    public string CustomerName { get; set; } = "";

    public string CustomerType { get; set; } = "";

    public decimal InsuranceAmount { get; set; }

    public decimal LoanLimit { get; set; }
}