namespace AiLearningApi.Models;

public class VectorDocument
{
    public string Id { get; set; } = "";

    public string Content { get; set; } = "";

    public float[] Embedding { get; set; } = [];

    public string Category { get; set; } = "";

    public string PolicyName { get; set; } = "";
    public string SourceDocument
    {
        get;
        set;
    } = string.Empty;
}