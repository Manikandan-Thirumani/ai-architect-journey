using AiLearningApi.Data;
using AiLearningApi.Models;

namespace AiLearningApi.Services;

public class RagTestRunner
{
    private readonly RagOrchestrator _orchestrator;
    private readonly RagEvaluationService _evaluator;

    public RagTestRunner(RagOrchestrator orchestrator, RagEvaluationService evaluator)
    {
        _orchestrator = orchestrator;
        _evaluator = evaluator;
    }

    public async Task<List<RagEvaluationResult>> RunTestsAsync()
    {
        var results = new List<RagEvaluationResult>();

        foreach (var test in RagTestData.Questions)
        {
            var response = await _orchestrator.ExecuteAsync(test.Question);

            var retrievalScore = response.Sources.Any()
                ? response.Sources.Max(x => x.Score)
                : 0;

            var answerScore = _evaluator.EvaluateAnswer(test.Expected, response.Answer);

            var finalScore = _evaluator.ComputeFinalScore(retrievalScore, answerScore);

            results.Add(new RagEvaluationResult
            {
                Question = test.Question,
                ExpectedAnswer = test.Expected,
                ActualAnswer = response.Answer,
                RetrievalScore = retrievalScore,
                AnswerScore = answerScore,
                FinalScore = finalScore
            });
        }

        return results;
    }
}