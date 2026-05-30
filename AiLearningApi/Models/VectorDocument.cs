namespace AiLearningApi.Models;

public class VectorDocument
{
    public int Id { get; set; }

    public string Content { get; set; }
        = string.Empty;

    public float[] Embedding { get; set; }
        = [];
}