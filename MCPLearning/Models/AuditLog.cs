namespace MCPLearning.Models;

public class AuditLog
{
    public DateTime Timestamp { get; set; }

    public string Role { get; set; } = "";

    public string ToolName { get; set; } = "";

    public string Arguments { get; set; } = "";

    public string Status { get; set; } = "";

    public string Message { get; set; } = "";
}