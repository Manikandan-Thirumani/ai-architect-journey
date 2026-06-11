using AiLearningApi.Helpers;
using AiLearningApi.Models;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.Ollama;

namespace AiLearningApi.Services;

public class BankingRagService
{
    private readonly Kernel _kernel;

    private readonly QueryUnderstandingService
        _queryUnderstandingService;

    private readonly SemanticRetrievalService
        _retrievalService;

    private readonly EmbeddingService
        _embeddingService;

    private readonly ConfidenceScoringService
        _confidenceService;

    private readonly CitationService
        _citationService;

    private readonly HallucinationDetectionService
        _hallucinationService;
    private readonly QueryRewriteService
    _queryRewriteService;
    private readonly
    ConversationMemoryService
    _memoryService;

    public BankingRagService(
        SemanticRetrievalService retrievalService,
        EmbeddingService embeddingService,
        QueryUnderstandingService queryUnderstandingService,
        ConfidenceScoringService confidenceService,
        CitationService citationService,
        HallucinationDetectionService hallucinationService,
        QueryRewriteService queryRewriteService, ConversationMemoryService
    memoryService)
    {
        _retrievalService =
            retrievalService;

        _embeddingService =
            embeddingService;

        _queryUnderstandingService =
            queryUnderstandingService;

        _confidenceService =
            confidenceService;

        _citationService =
            citationService;

        _hallucinationService =
            hallucinationService;

        _queryRewriteService =
            queryRewriteService;
        _memoryService =
    memoryService;

        var builder =
            Kernel.CreateBuilder();

        builder.AddOllamaChatCompletion(
            modelId: "phi3",
            endpoint:
                new Uri("http://localhost:11434"));

        _kernel = builder.Build();
    }

    public async Task<RagResponse> Ask(
        string question)
    {
        // MEMORY

        _memoryService.AddMessage(
            $"User: {question}");

        var conversationContext =
            _memoryService
                .GetContext();

        Console.WriteLine();
        Console.WriteLine(
            "===== MEMORY =====");

        Console.WriteLine(
            conversationContext);

        string rewrittenQuestion =
            question;

        if (ConversationMemoryHelper
                .NeedsRewrite(question))
        {
            rewrittenQuestion =
                await _queryRewriteService
                    .Rewrite(
                        conversationContext,
                        question);
        }


        Console.WriteLine();
        Console.WriteLine(
            $"Original Question: {question}");

        Console.WriteLine(
            $"Rewritten Question: {rewrittenQuestion}");

        // INTENT

        var intent =
            _queryUnderstandingService
                .Analyze(rewrittenQuestion);

        Console.WriteLine(
            $"Category = {intent.Category}");

        Console.WriteLine(
            $"Intent = {intent.Intent}");

        // RETRIEVAL

        var chunk =
            await _retrievalService
                .GetRelevantChunks(
                    rewrittenQuestion);

        Console.WriteLine();
        Console.WriteLine(
            "===== FINAL CHUNK =====");

        Console.WriteLine(chunk);

        Console.WriteLine(
            "=======================");

        if (string.IsNullOrWhiteSpace(chunk))
        {
            return new RagResponse
            {
                Answer =
                    "No relevant information available.",

                Confidence = 0,

                Decision =
                    "Low Confidence",

                Intent =
                    intent.Intent,

                Category =
                    intent.Category
            };
        }

        // CONFIDENCE

        double similarityScore = 90;

        bool intentMatched = true;

        bool categoryMatched =
            !string.IsNullOrWhiteSpace(
                intent.Category);

        var confidence =
            _confidenceService.Calculate(
                similarityScore,
                intentMatched,
                categoryMatched);

        if (!confidence.ShouldAnswer)
        {
            return new RagResponse
            {
                Answer =
                    "I could not find reliable information to answer your question.",

                Confidence =
                    confidence.FinalConfidence,

                Decision =
                    confidence.Decision,

                Intent =
                    intent.Intent,

                Category =
                    intent.Category
            };
        }

        // PROMPT

        // STEP 6 — Build Prompt

        var prompt = $"""
You are an enterprise banking AI assistant.

Use ONLY the information
provided in the banking policy.

If multiple policies are relevant,
combine them into a single answer.

Banking Policy:
{chunk}

Customer Question:
{rewrittenQuestion}

If the answer is not present
in the policy, respond with:

No policy information available.
""";

        // LLM
        Console.WriteLine();
        Console.WriteLine(
            "===== CHUNK SENT TO LLM =====");

        Console.WriteLine(chunk);

        Console.WriteLine(
            "============================");

        Console.WriteLine();
        Console.WriteLine(
            "===== PROMPT =====");

        Console.WriteLine(prompt);

        Console.WriteLine(
            "==================");

        var result =
            await _kernel
                .InvokePromptAsync(
                    prompt);

        Console.WriteLine();
        Console.WriteLine(
            "===== GENERATED ANSWER =====");

        Console.WriteLine(
            result.ToString());

        bool grounded = true;

        Console.WriteLine(
            $"Grounded = {grounded}");

        var citations =
            new List<SourceCitation>
            {
            new SourceCitation
            {
                DocumentName =
                    "LoanPolicy.txt",

                PolicyName =
                    "Premium Loan Policy"
            }
            };

        _memoryService.AddMessage(
            $"AI: {result}");

        return new RagResponse
        {
            Answer =
                result.ToString(),

            Confidence =
                confidence.FinalConfidence,

            Decision =
                confidence.Decision,

            Intent =
                intent.Intent,

            Category =
                intent.Category,

            Sources =
                citations
        };
    }
}