namespace DatabaseMcpServer.Models;

public class ColumnInfo
{
    public string TableName { get; set; } = string.Empty;

    public string ColumnName { get; set; } = string.Empty;

    public string DataType { get; set; } = string.Empty;

    public bool IsNullable { get; set; }
}