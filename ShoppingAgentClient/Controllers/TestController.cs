using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

namespace ShoppingAgentClient.Controllers;

[ApiController]
[Route("api/test")]
public class TestController : ControllerBase
{
    [HttpGet]
    public async Task<object> Test()
    {
        using var client = new HttpClient();

        client.Timeout =
            TimeSpan.FromMinutes(10);

        var request =
            new
            {
                model = "qwen2.5:7b",
                prompt =
                    "Return ONLY JSON {\"tool\":\"search_products\"}",
                stream = false
            };

        var response =
            await client.PostAsJsonAsync(
                "http://localhost:11434/api/generate",
                request);

        var result =
            await response
                .Content
                .ReadAsStringAsync();

        return new
        {
            response.StatusCode,
            Result = result
        };
    }
}