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

    public BankingRagService(
        SemanticRetrievalService retrievalService,
        EmbeddingService embeddingService,
        QueryUnderstandingService queryUnderstandingService,
        ConfidenceScoringService confidenceService, CitationService citationService)
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
        // STEP 1 — Understand Query

        var intent =
            _queryUnderstandingService
                .Analyze(question);

        Console.WriteLine(
            $"Category = {intent.Category}");

        Console.WriteLine(
            $"Intent = {intent.Intent}");

        // STEP 2 — Retrieve Relevant Chunks

        var chunk =
            await _retrievalService
                .GetRelevantChunks(question);

        // STEP 3 — No Retrieval Result

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

        // STEP 4 — Confidence Calculation

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

        // STEP 5 — Reject Low Confidence Queries

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
{question}

If the answer is not present
in the policy, respond with:

No policy information available.
""";

        // STEP 7 — Generate Grounded Answer

        var result =
            await _kernel
                .InvokePromptAsync(prompt);



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