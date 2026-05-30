using AiLearningApi.Models;

namespace AiLearningApi.Services;

public class ChunkRetrievalService
{
    private readonly List<DocumentChunk> _chunks;

    private static readonly HashSet<string> StopWords =
    [
        "what",
        "is",
        "the",
        "and",
        "for",
        "a",
        "an",
        "of",
        "to",
        "in"
    ];

    public ChunkRetrievalService(
        PdfKnowledgeService pdfKnowledgeService)
    {
        _chunks = pdfKnowledgeService.GetChunks();

        Console.WriteLine(
            $"Loaded {_chunks.Count} chunks");
    }

    public string GetRelevantChunks(
        string question)
    {
        var questionWords =
            question
                .ToLower()
                .Split(
                    [' ', '.', ',', '?'],
                    StringSplitOptions.RemoveEmptyEntries)
                .Where(x => !StopWords.Contains(x))
                .Distinct()
                .ToList();

        Console.WriteLine();
        Console.WriteLine("QUESTION WORDS");

        foreach (var word in questionWords)
        {
            Console.WriteLine(word);
        }

        var scoredChunks =
            new List<(DocumentChunk Chunk, int Score)>();

        foreach (var chunk in _chunks)
        {
            int score = 0;

            string chunkText =
                chunk.Content.ToLower();

            foreach (var word in questionWords)
            {
                if (chunkText.Contains(word))
                {
                    score++;
                }
            }

            Console.WriteLine(
                $"Chunk {chunk.Id} Score = {score}");

            scoredChunks.Add(
                (chunk, score));
        }

        var topChunks =
            scoredChunks
                .Where(x => x.Score > 0)
                .OrderByDescending(x => x.Score)
                .Select(x => x.Chunk.Content)
                .Distinct()
                .Take(2)
                .ToList();

        Console.WriteLine();
        Console.WriteLine(
            "===== RETRIEVED CHUNKS =====");

        foreach (var chunk in topChunks)
        {
            Console.WriteLine(chunk);
            Console.WriteLine(
                "-------------------------");
        }

        Console.WriteLine(
            "==========================");
        Console.WriteLine();

        return string.Join(
            "\n\n-----------------\n\n",
            topChunks);
    }
}