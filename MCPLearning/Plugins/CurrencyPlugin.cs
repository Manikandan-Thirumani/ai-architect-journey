using MCPLearning.Models;
using Microsoft.SemanticKernel;
using System.Net.Http.Json;

namespace MCPLearning.Plugins;

public class CurrencyPlugin
{
    private readonly HttpClient _httpClient;

    public CurrencyPlugin(
        HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    [KernelFunction]
    public async Task<string> GetUsdToInrRate()
    {
        var response =
            await _httpClient.GetFromJsonAsync<
                ExchangeRateResponse>(
                    "https://open.er-api.com/v6/latest/USD");

        if (response == null)
        {
            return "Exchange rate unavailable.";
        }

        if (!response.Rates.TryGetValue(
                "INR",
                out var rate))
        {
            return "INR exchange rate unavailable.";
        }

        return $"1 USD = ₹{rate}";
    }
}