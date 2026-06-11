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

        Console.WriteLine();
        Console.WriteLine(
            "====================================");

        Console.WriteLine(
            $"Found {files.Length} files");

        Console.WriteLine(
            "====================================");

        foreach (var file in files)
        {
            Console.WriteLine();
            Console.WriteLine(
                $"Processing File = {file}");

            var content =
                await File.ReadAllTextAsync(
                    file);

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

            var document =
                new VectorDocument
                {
                    Id =
                        Guid.NewGuid()
                            .ToString(),

                    Content =
                        content,

                    Embedding =
                        embedding,

                    Category =
                        category,

                    PolicyName =
                        policyName,

                    SourceDocument =
                        Path.GetFileName(
                            file)
                };

            vectorStore.Add(
                document);

            Console.WriteLine(
                $"SourceDocument = {document.SourceDocument}");

            Console.WriteLine(
                $"PolicyName = {document.PolicyName}");

            Console.WriteLine(
                $"Category = {document.Category}");

            Console.WriteLine(
                "Content Preview:");

            Console.WriteLine(
                document.Content.Length > 200
                    ? document.Content[..200]
                    : document.Content);

            Console.WriteLine(
                "====================================");
        }

        Console.WriteLine();
        Console.WriteLine(
            "Vector Initialization Completed");

        Console.WriteLine(
            "====================================");
    }
}