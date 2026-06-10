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
        var files =
            Directory.GetFiles(
                "KnowledgeBase",
                "*.txt");

        foreach (var file in files)
        {
            Console.WriteLine(
                $"Processing {file}");

            var content =
                await File.ReadAllTextAsync(
                    file);

            // One file = one chunk for now

            var embedding =
                await embeddingService
                    .GenerateEmbedding(
                        content);

            var category =
                categoryService
                    .GetCategory(
                        content);

            var policyName =
                policyExtractor
                    .ExtractPolicyName(
                        content);

            vectorStore.Add(
                new VectorDocument
                {
                    Id = Guid.NewGuid()
                        .ToString(),

                    Content = content,

                    Embedding = embedding,

                    Category = category,

                    PolicyName = policyName,

                    SourceDocument =
                        Path.GetFileName(
                            file)
                });
        }
    }
}