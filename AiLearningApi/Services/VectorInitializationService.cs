using AiLearningApi.Models;

namespace AiLearningApi.Services;

public class VectorInitializationService
{
    public static async Task Initialize(
        PdfKnowledgeService pdfService,
        EmbeddingService embeddingService,
        VectorStoreService vectorStore,
        CategoryService categoryService)
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

            var category =
                await categoryService
                    .DetectCategory(
                        chunk.Content);

            Console.WriteLine(
                $"Chunk {chunk.Id} Category = {category}");

            vectorStore.Add(
                new VectorDocument
                {
                    Id = chunk.Id,
                    Content = chunk.Content,
                    Embedding = embedding,
                    Category = category
                });
        }

        Console.WriteLine(
            "Vector store initialized");
    }
}