using AiLearningApi.Models;

namespace AiLearningApi.Services.Retrieval;

public class KeywordSearchService
{
    private readonly List<RetrievedChunk> _allChunks;

    public KeywordSearchService()
    {
        _allChunks = new List<RetrievedChunk>();
    }

    public void LoadChunks(
        List<RetrievedChunk> chunks)
    {
        _allChunks.Clear();
        _allChunks.AddRange(chunks);
    }

    public Task<List<RetrievedChunk>> SearchAsync(
        string query)
    {
        var words = query
            .ToLower()
            .Split(' ',
                StringSplitOptions.RemoveEmptyEntries);

        var results = _allChunks
            .Select(chunk =>
            {
                int matches =
                    words.Count(word =>
                        chunk.Content
                            .ToLower()
                            .Contains(word));

                return new
                {
                    Chunk = chunk,
                    Score = matches
                };
            })
            .Where(x => x.Score > 0)
            .OrderByDescending(x => x.Score)
            .Take(10)
            .Select(x =>
            {
                x.Chunk.Score = x.Score;
                return x.Chunk;
            })
            .ToList();

        return Task.FromResult(results);
    }
}