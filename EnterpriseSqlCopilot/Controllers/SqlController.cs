using EnterpriseSqlCopilot.Models;
using EnterpriseSqlCopilot.Services;
using Microsoft.AspNetCore.Mvc;

namespace EnterpriseSqlCopilot.Controllers;

[ApiController]
[Route("api/sql")]
public class SqlController : ControllerBase
{
    private readonly SqlGenerationService
        _generator;

    public SqlController(
        SqlGenerationService generator)
    {
        _generator = generator;
    }

    [HttpPost("generate")]
    public async Task<IActionResult> Generate(
        SqlRequest request)
    {
        var sql =
            await _generator
                .GenerateSqlAsync(
                    request.Question);

        return Ok(
            new SqlResponse
            {
                Question = request.Question,
                GeneratedSql = sql
            });
    }
}