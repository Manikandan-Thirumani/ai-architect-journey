namespace EnterpriseSqlCopilot.Models;

public class SqlResponse
{
    public string Question { get; set; } = "";

    public string GeneratedSql { get; set; } = "";
}