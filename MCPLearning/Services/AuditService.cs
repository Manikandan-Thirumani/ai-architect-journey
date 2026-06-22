using MCPLearning.Models;
using System.Text.Json;

namespace MCPLearning.Services;

public class AuditService
{
    private readonly List<AuditLog> _logs = new();

    public void Log(
        string role,
        string toolName,
        Dictionary<string, object> arguments,
        string status,
        string message)
    {
        var entry =
            new AuditLog
            {
                Timestamp = DateTime.UtcNow,

                Role = role,

                ToolName = toolName,

                Arguments =
                    JsonSerializer.Serialize(arguments),

                Status = status,

                Message = message
            };

        _logs.Add(entry);

        Console.WriteLine(
            $"AUDIT | {entry.Timestamp:u} | " +
            $"{role} | {toolName} | " +
            $"{status} | {message}");
    }

    public IReadOnlyList<AuditLog> GetLogs()
    {
        return _logs.AsReadOnly();
    }
}