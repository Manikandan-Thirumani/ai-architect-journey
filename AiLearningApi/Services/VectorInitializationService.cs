using AiLearningApi.Models;

namespace AiLearningApi.Services;

public class VectorInitializationService
{
    public static async Task Initialize(
        PdfKnowledgeService pdfService,
        EmbeddingService embeddingService,
        VectorStoreService vectorStore,
        PolicyExtractorService policyExtractor,
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

            var policyName =
                policyExtractor
                    .ExtractPolicyName(
                        chunk.Content);

            var category =
                categoryService
                    .GetCategory(
                        policyName);

            Console.WriteLine(
                $"Policy = {policyName}");

            Console.WriteLine(
                $"Category = {category}");

            vectorStore.Add(
                new VectorDocument
                {
                    Id = chunk.Id.ToString(),
                    Content = chunk.Content,
                    Embedding = embedding,
                    Category = category,
                    PolicyName = policyName
                });
        }

        Console.WriteLine(
            "Vector store initialized");
    }
}