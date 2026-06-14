namespace AiLearningApi.Models;

public class RetrievedChunk
{
    public string ChunkId { get; set; } = Guid.NewGuid().ToString();

    public string Content
    {
        get;
        set;
    } = string.Empty;

    public string SourceDocument
    {
        get;
        set;
    } = string.Empty;

    public string PolicyName
    {
        get;
        set;
    } = string.Empty;

    public double Score
    {
        get;
        set;
    }
}