using AiLearningApi.Models;

namespace AiLearningApi.Services;

public class RagOrchestrator
{
    private readonly SemanticRetriever _retriever;
    private readonly ContextBuilderService _contextBuilder;
    private readonly PromptBuilderService _promptBuilder;
    private readonly LlmService _llm;
    private readonly GroundingValidator _validator;
    private readonly RagLogger _logger;
    private readonly ContextOptimizer _optimizer;

    public RagOrchestrator(
        SemanticRetriever retriever,
        ContextBuilderService contextBuilder,
        PromptBuilderService promptBuilder,
        LlmService llm,
        GroundingValidator validator,
        RagLogger logger,
        ContextOptimizer optimizer)
    {
        _retriever = retriever;
        _contextBuilder = contextBuilder;
        _promptBuilder = promptBuilder;
        _llm = llm;
        _validator = validator;
        _logger = logger;
        _optimizer = optimizer;
    }

    public async Task<RagResponse> ExecuteAsync(string question)
    {
        _logger.SetQuestion(question);

        // 1. Retrieve from Qdrant
        var chunks =
            await _retriever.Retrieve(question);

        chunks =
            _optimizer.Optimize(chunks);

        _logger.AddStep(
            $"Retrieved {chunks.Count} chunks");

        // 2. Build Context
        var context =
            _contextBuilder.BuildContext(chunks);

        _logger.AddStep(
            "Context built");

        // 3. Build Prompt
        var prompt =
            _promptBuilder.BuildPrompt(
                context,
                question);

        _logger.AddStep(
            "Prompt created");

        // 4. LLM Call
        var rawAnswer =
            await _llm.GetAnswerAsync(prompt);

        _logger.AddStep(
            "LLM response received");

        // 5. Validation
        var confidence =
            _validator.CalculateConfidence(
                chunks);

        var isGrounded =
            _validator.IsGrounded(
                rawAnswer,
                chunks);

        _logger.AddStep(
            $"Confidence: {confidence}, Grounded: {isGrounded}");

        return new RagResponse
        {
            Answer = rawAnswer,

            Confidence = confidence,

            IsGrounded = isGrounded,

            Sources = chunks
        .OrderByDescending(x => x.Score)
        .Take(5)
        .Select(x => new SourceCitation
        {
            DocumentName = x.SourceDocument,

            PolicyName = x.PolicyName,

            SourceDocument = x.SourceDocument,

            Score = x.Score
        })
        .ToList()
        };
    }
}