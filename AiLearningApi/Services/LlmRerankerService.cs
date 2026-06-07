using System.Text.Json;
using AiLearningApi.Models;

namespace AiLearningApi.Services;

public class LlmRerankerService
{
    private readonly OllamaService
        _ollamaService;

    public LlmRerankerService(
        OllamaService ollamaService)
    {
        _ollamaService =
            ollamaService;
    }

    public async Task<List<RerankChunkResult>>
        Rerank(
            string question,
            List<VectorDocument> chunks)
    {
        var chunkText =
            string.Join(
                "\n\n",
                chunks.Select(
                    x =>
$"""
ChunkId:{x.Id}

{x.Content}
"""));

        var prompt =
        $@"You are a document reranker.

Question:
{question}

Rank chunks from most relevant to least relevant.

Return JSON ONLY.

Example:

[
  {{
      ""ChunkId"":""1"",
      ""Score"":100
  }},
  {{
      ""ChunkId"":""2"",
      ""Score"":50
  }}
]

Chunks:

{chunkText}";

        var response =
            await _ollamaService
                .AskAi(prompt);

        try
        {
            var start =
                response.IndexOf('[');

            var end =
                response.LastIndexOf(']');

            if (start >= 0 &&
                end > start)
            {
                response =
                    response.Substring(
                        start,
                        end - start + 1);
            }

            var options =
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

            return JsonSerializer
                .Deserialize<
                    List<RerankChunkResult>>(
                        response,
                        options)
                   ?? [];
        }
        catch
        {
            return [];
        }
    }
}