using EnterpriseSqlCopilot.Services;
using Microsoft.AspNetCore.Mvc;

namespace EnterpriseSqlCopilot.Controllers;

[ApiController]
[Route("api/schema")]
public class SchemaController
    : ControllerBase
{
    private readonly SchemaRepository
        _repository;

    private readonly SchemaDiscoveryService
        _discovery;

    public SchemaController(
        SchemaRepository repository,
        SchemaDiscoveryService discovery)
    {
        _repository = repository;
        _discovery = discovery;
    }

    [HttpGet("tables")]
    public async Task<IActionResult>
        GetTables()
    {
        var tables =
            await _repository.GetTablesAsync();

        return Ok(tables);
    }

    [HttpGet("columns")]
    public async Task<IActionResult>
        GetColumns()
    {
        var columns =
            await _repository.GetColumnsAsync();

        return Ok(columns);
    }

    [HttpGet("description")]
    public async Task<IActionResult>
        GetDescription()
    {
        var schema =
            await _discovery
                .GetSchemaDescriptionAsync();

        return Ok(schema);
    }
}