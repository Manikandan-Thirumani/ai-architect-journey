using AiLearningApi.Models;

namespace AiLearningApi.Services;

public static class VectorInitializationService
{
    public static async Task Initialize(
        PdfKnowledgeService pdfService,
        EmbeddingService embeddingService,
        QdrantVectorStoreService qdrantStore,
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

        bool collectionCreated = false;

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

            if (!collectionCreated)
            {
                await qdrantStore
                    .CreateCollectionAsync(
                        embedding.Length);

                collectionCreated = true;
            }

            var category =
                categoryService
                    .GetCategory(
                        content);

            var policyName =
                policyExtractor
                    .ExtractPolicyName(
                        content);

            await qdrantStore.InsertAsync(
                Guid.NewGuid().ToString(),
                embedding,
                content,
                policyName,
                Path.GetFileName(file));

            Console.WriteLine(
                $"SourceDocument = {Path.GetFileName(file)}");

            Console.WriteLine(
                $"PolicyName = {policyName}");

            Console.WriteLine(
                $"Category = {category}");

            Console.WriteLine(
                "Content Preview:");

            Console.WriteLine(
                content.Length > 200
                    ? content[..200]
                    : content);

            Console.WriteLine(
                "====================================");
        }

        Console.WriteLine();
        Console.WriteLine(
            "Qdrant Initialization Completed");

        Console.WriteLine(
            "====================================");
    }
}