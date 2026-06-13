using AiLearningApi.Models;
using System.Text;

namespace AiLearningApi.Services;

public class ContextBuilderService
{
    private const int MaxChunks = 5;

    public string BuildContext(List<RetrievedChunk> chunks)
    {
        var ordered = chunks
            .OrderByDescending(x => x.Score)
            .Take(MaxChunks)
            .ToList();

        var sb = new StringBuilder();

        foreach (var c in ordered)
        {
            sb.AppendLine($"[DOC | Score: {c.Score:F2} | Policy: {c.PolicyName} | Source: {c.SourceDocument}]");
            sb.AppendLine(c.Content);
            sb.AppendLine("-----");
        }

        return sb.ToString();
    }

    // ✅ FIXED: use SourceCitation instead of RagSource
    public List<SourceCitation> ExtractSources(List<RetrievedChunk> chunks)
    {
        return chunks
            .OrderByDescending(x => x.Score)
            .Take(MaxChunks)
            .Select(x => new SourceCitation
            {
                PolicyName = x.PolicyName,
                DocumentName = x.SourceDocument,
                Score = x.Score
            })
            .ToList();
    }
}