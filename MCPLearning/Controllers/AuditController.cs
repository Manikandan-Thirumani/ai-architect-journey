using MCPLearning.Services;
using Microsoft.AspNetCore.Mvc;

namespace MCPLearning.Controllers;

[ApiController]
[Route("api/audit")]
public class AuditController : ControllerBase
{
    private readonly AuditService _audit;

    public AuditController(
        AuditService audit)
    {
        _audit = audit;
    }

    [HttpGet]
    public IActionResult Get()
    {
        return Ok(
            _audit.GetLogs());
    }
}