using AiLearningApi.Models;

namespace AiLearningApi.Services;

public class VectorInitializationService
{
    public static async Task Initialize(
        PdfKnowledgeService pdfService,
        EmbeddingService embeddingService,
        VectorStoreService vectorStore)
    {
        var chunks =
            pdfService.GetChunks();

        foreach (var chunk in chunks)
        {
            Console.WriteLine(
                $"Embedding Chunk {chunk.Id}");

            var embedding =
                await embeddingService
                    .GenerateEmbedding(
                        chunk.Content);

            vectorStore.Add(
                new VectorDocument
                {
                    Id = chunk.Id,
                    Content = chunk.Content,
                    Embedding = embedding
                });
        }

        Console.WriteLine(
            "Vector store initialized");
    }
}