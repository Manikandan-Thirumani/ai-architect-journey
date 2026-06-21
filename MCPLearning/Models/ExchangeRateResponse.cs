namespace MCPLearning.Models;

public class ExchangeRateResponse
{
    public string Base_Code { get; set; } = "";

    public Dictionary<string, decimal> Rates { get; set; }
        = new();
}