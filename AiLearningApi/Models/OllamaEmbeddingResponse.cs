using System.Text.Json.Serialization;

namespace AiLearningApi.Models;

public class OllamaEmbeddingResponse
{
    [JsonPropertyName("embedding")]
    public float[] Embedding { get; set; }
        = [];
}