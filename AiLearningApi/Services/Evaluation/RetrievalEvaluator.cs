using AiLearningApi.Data;
using AiLearningApi.Models;
using AiLearningApi.Services.Retrieval;

namespace AiLearningApi.Services.Evaluation;

public class RetrievalEvaluator
{
    private readonly HybridSearchService
        _hybridSearch;

    public RetrievalEvaluator(
        HybridSearchService hybridSearch)
    {
        _hybridSearch = hybridSearch;
    }

    public async Task<List<EvaluationResult>>
        EvaluateAsync()
    {
        var results =
            new List<EvaluationResult>();

        foreach (var testCase in
                 RetrievalTestCases.GetCases())
        {
            var retrieved =
                await _hybridSearch
                    .SearchAsync(
                        testCase.Question);

            var docs =
                retrieved
                    .Select(x =>
                        x.SourceDocument)
                    .Distinct()
                    .ToList();

            var relevantRetrieved =
                docs.Intersect(
                    testCase.ExpectedDocuments)
                    .Count();

            double recall =
                (double)relevantRetrieved /
                testCase.ExpectedDocuments.Count;

            double precision =
                docs.Count == 0
                ? 0
                : (double)relevantRetrieved /
                  docs.Count;

            double mrr = 0;

            for (int i = 0;
                 i < docs.Count;
                 i++)
            {
                if (testCase.ExpectedDocuments
                    .Contains(docs[i]))
                {
                    mrr = 1.0 / (i + 1);
                    break;
                }
            }

            results.Add(
                new EvaluationResult
                {
                    Question =
                        testCase.Question,

                    Recall = recall,

                    Precision = precision,

                    Mrr = mrr,

                    RetrievedDocuments =
                        docs
                });
        }

        return results;
    }
}