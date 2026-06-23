namespace EnterpriseSqlCopilot.Models;

public class QueryResponse
{
    public string Question { get; set; } = "";

    public string GeneratedSql { get; set; } = "";

    public object Results { get; set; } = new();
}