using System.Linq;
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

    private readonly ConversationMemoryService
        _memoryService;

    public BankingRagService(
        SemanticRetrievalService retrievalService,
        EmbeddingService embeddingService,
        QueryUnderstandingService queryUnderstandingService,
        ConfidenceScoringService confidenceService,
        CitationService citationService,
        HallucinationDetectionService hallucinationService,
        QueryRewriteService queryRewriteService,
        ConversationMemoryService memoryService)
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
        // ===================================
        // MEMORY
        // ===================================

        var conversationContext =
            _memoryService.GetContext();

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
            "===== REWRITTEN QUESTION =====");

        Console.WriteLine(
            rewrittenQuestion);

        Console.WriteLine(
            "==============================");

        _memoryService.AddMessage(
            $"User: {question}");

        // ===================================
        // QUERY UNDERSTANDING
        // ===================================

        var intent =
            _queryUnderstandingService
                .Analyze(rewrittenQuestion);

        Console.WriteLine(
            $"Category = {intent.Category}");

        Console.WriteLine(
            $"Intent = {intent.Intent}");

        // ===================================
        // RETRIEVAL
        // ===================================

        var retrievalResults =
            await _retrievalService
                .GetRelevantChunks(
                    rewrittenQuestion);

        if (!retrievalResults.Any())
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

        var chunk =
            string.Join(
                "\n\n-----------------\n\n",
                retrievalResults
                    .Select(x => x.Content));

        Console.WriteLine();
        Console.WriteLine(
            "===== FINAL CHUNK =====");

        Console.WriteLine(chunk);

        Console.WriteLine(
            "=======================");

        // ===================================
        // CONFIDENCE
        // ===================================

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

        // ===================================
        // PROMPT
        // ===================================

        var prompt = $"""
You are an enterprise banking AI assistant.

STRICT RULES:

1. Use ONLY information from Banking Policy.
2. Do NOT add explanations.
3. Do NOT add assumptions.
4. Do NOT add recommendations.
5. Do NOT add banking knowledge.
6. Copy policy facts exactly.
7. Answer in one sentence.
8. If answer not found return exactly:

No policy information available.

Banking Policy:
{chunk}

Customer Question:
{rewrittenQuestion}

Answer:
""";

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

        // ===================================
        // LLM
        // ===================================

        var result =
            await _kernel
                .InvokePromptAsync(
                    prompt);

        var answer =
            result.ToString();

        Console.WriteLine();
        Console.WriteLine(
            "===== GENERATED ANSWER =====");

        Console.WriteLine(answer);

        // ===================================
        // HALLUCINATION CHECK
        // ===================================

        bool grounded = true;

        /*
        grounded =
            await _hallucinationService
                .IsGrounded(
                    answer,
                    chunk);
        */

        Console.WriteLine(
            $"Grounded = {grounded}");

        if (!grounded)
        {
            return new RagResponse
            {
                Answer =
                    "The answer could not be verified from policy documents.",

                Confidence = 0,

                Decision =
                    "Hallucination Detected",

                Intent =
                    intent.Intent,

                Category =
                    intent.Category,

                Sources =
                    new List<SourceCitation>()
            };
        }

        // ===================================
        // CITATIONS
        // ===================================

        var citations =
            retrievalResults
                .Select(x =>
                    new SourceCitation
                    {
                        DocumentName =
                            x.DocumentName,

                        PolicyName =
                            x.PolicyName
                    })
                .DistinctBy(x =>
                    new
                    {
                        x.DocumentName,
                        x.PolicyName
                    })
                .ToList();

        // ===================================
        // MEMORY SAVE
        // ===================================

        _memoryService.AddMessage(
            $"AI: {answer}");

        // ===================================
        // RESPONSE
        // ===================================

        return new RagResponse
        {
            Answer = answer,

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