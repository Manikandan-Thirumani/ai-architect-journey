using AiLearningApi.Models;

namespace AiLearningApi.Services;

public class CitationService
{
    public List<SourceCitation>
        Build(
            List<RetrievedChunk>
                chunks)
    {
        return chunks
            .Select(x =>
                new SourceCitation
                {
                    DocumentName =
                        x.SourceDocument,

                    PolicyName =
                        x.PolicyName
                })
            .DistinctBy(x =>
                x.DocumentName)
            .ToList();
    }
}