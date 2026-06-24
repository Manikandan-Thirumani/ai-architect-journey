namespace EnterpriseSqlCopilot.Services;

public class SqlGuardrailService
{
    private static readonly string[] ForbiddenKeywords =
    [
        "INSERT",
        "UPDATE",
        "DELETE",
        "DROP",
        "ALTER",
        "TRUNCATE",
        "EXEC",
        "MERGE"
    ];

    public void Validate(string sql)
    {
        if (string.IsNullOrWhiteSpace(sql))
        {
            throw new Exception(
                "Generated SQL is empty.");
        }

        var normalized =
            sql.ToUpperInvariant();

        foreach (var keyword in ForbiddenKeywords)
        {
            if (normalized.Contains(keyword))
            {
                throw new Exception(
                    $"Forbidden SQL detected: {keyword}");
            }
        }

        if (!normalized.TrimStart()
                .StartsWith("SELECT"))
        {
            throw new Exception(
                "Only SELECT statements are allowed.");
        }
    }
}