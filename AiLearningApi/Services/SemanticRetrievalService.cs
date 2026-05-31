namespace AiLearningApi.Services;

public class SemanticRetrievalService
{
    private readonly EmbeddingService
        _embeddingService;

    private readonly VectorStoreService
        _vectorStore;
    private readonly RerankerService
    _reranker;

    public SemanticRetrievalService(
        EmbeddingService embeddingService,
        VectorStoreService vectorStore,
        RerankerService reranker)
    {
        _embeddingService =
            embeddingService;

        _vectorStore =
            vectorStore;

        _reranker =
            reranker;
    }

    public async Task<string>
      GetRelevantChunks(
          string question)
    {
        var questionVector =
            await _embeddingService
                .GenerateEmbedding(question);

        var vectorResults =
            _vectorStore.Search(
                questionVector,
                20);

        Console.WriteLine();
        Console.WriteLine(
            "===== VECTOR RESULTS =====");

        foreach (var item in vectorResults)
        {
            Console.WriteLine(
                $"Vector Score = {item.Score:F4}");

            Console.WriteLine(
                item.Document.Content);

            Console.WriteLine(
                "---------------------");
        }

        var chunks =
            vectorResults
                .Select(x =>
                    x.Document.Content)
                .ToList();

        var reranked =
            _reranker.Rerank(
                question,
                chunks);

        Console.WriteLine();
        Console.WriteLine(
            "===== RERANK RESULTS =====");

        foreach (var item in reranked)
        {
            Console.WriteLine(
                $"Rerank Score = {item.Score}");

            Console.WriteLine(
                item.Content);

            Console.WriteLine(
                "---------------------");
        }

        var topChunks =
            reranked
                .Take(3)
                .Select(x => x.Content)
                .Distinct()
                .ToList();

        if (!topChunks.Any())
        {
            return "";
        }

        Console.WriteLine();
        Console.WriteLine(
            "===== FINAL CHUNKS =====");

        foreach (var chunk in topChunks)
        {
            Console.WriteLine(chunk);

            Console.WriteLine(
                "---------------------");
        }

        return string.Join(
            "\n\n-----------------\n\n",
            topChunks);
    }
    private string GetQuestionCategory(
    string question)
    {
        question = question.ToLower();

        if (question.Contains("loan"))
            return "Loan";

        if (question.Contains("credit"))
            return "CreditCard";

        if (question.Contains("deposit"))
            return "Deposit";

        if (question.Contains("fraud"))
            return "Fraud";

        return "General";
    }
}