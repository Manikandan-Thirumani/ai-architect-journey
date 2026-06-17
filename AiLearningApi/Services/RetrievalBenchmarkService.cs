using AiLearningApi.Data;
using AiLearningApi.Models;
using AiLearningApi.Services.Retrieval;

namespace AiLearningApi.Services.Evaluation;

public class RetrievalBenchmarkService
{
    private readonly ISemanticRetriever _semanticRetriever;
    private readonly HybridSearchService _hybridSearch;

    public RetrievalBenchmarkService(
        ISemanticRetriever semanticRetriever,
        HybridSearchService hybridSearch)
    {
        _semanticRetriever = semanticRetriever;
        _hybridSearch = hybridSearch;
    }

    public async Task<BenchmarkDashboard>
        GenerateDashboardAsync()
    {
        var dashboard = new BenchmarkDashboard();

        dashboard.Results.Add(
            await EvaluateStrategyAsync(
                "Vector Search",
                q => _semanticRetriever
                    .RetrieveAsync(q)));

        dashboard.Results.Add(
            await EvaluateStrategyAsync(
                "Hybrid Search",
                q => _hybridSearch
                    .SearchAsync(q)));

        return dashboard;
    }

    private async Task<BenchmarkResult>
        EvaluateStrategyAsync(
            string strategy,
            Func<string,
            Task<List<RetrievedChunk>>> retrieval)
    {
        double recallTotal = 0;
        double precisionTotal = 0;
        double mrrTotal = 0;

        var cases =
            RetrievalTestCases.GetCases();

        foreach (var testCase in cases)
        {
            var retrieved =
                await retrieval(
                    testCase.Question);

            var docs =
                retrieved
                    .Select(x => x.SourceDocument)
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

            for (int i = 0; i < docs.Count; i++)
            {
                if (testCase.ExpectedDocuments
                    .Contains(docs[i]))
                {
                    mrr = 1.0 / (i + 1);
                    break;
                }
            }

            recallTotal += recall;
            precisionTotal += precision;
            mrrTotal += mrr;
        }

        return new BenchmarkResult
        {
            Strategy = strategy,

            AverageRecall =
                recallTotal / cases.Count,

            AveragePrecision =
                precisionTotal / cases.Count,

            AverageMrr =
                mrrTotal / cases.Count
        };
    }
}