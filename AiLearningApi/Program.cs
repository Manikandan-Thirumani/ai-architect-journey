using AiLearningApi.Middleware;
using AiLearningApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
// external services
builder.Services.AddScoped<OpenAiService>();
builder.Services.AddHttpClient<OllamaService>();

// core AI services
builder.Services.AddScoped<RagService>();
builder.Services.AddScoped<RagOrchestrator>();
builder.Services.AddScoped<SemanticRetriever>();

builder.Services.AddScoped<ContextBuilderService>();
builder.Services.AddScoped<PromptBuilderService>();
builder.Services.AddScoped<LlmService>();

builder.Services.AddScoped<GroundingValidator>();
builder.Services.AddScoped<ContextOptimizer>();
builder.Services.AddScoped<RagEvaluationService>();
builder.Services.AddScoped<RagTestRunner>();
builder.Services.AddScoped<RagLogger>();

// knowledge + vector (these CAN be singleton)
builder.Services.AddSingleton<KnowledgeService>();
builder.Services.AddSingleton<VectorStoreService>();
builder.Services.AddSingleton<CategoryService>();
builder.Services.AddSingleton<PolicyExtractorService>();
builder.Services.AddSingleton<QueryUnderstandingService>();
builder.Services.AddSingleton<QueryRewriteService>();
builder.Services.AddSingleton<ConversationMemoryService>();
builder.Services.AddSingleton<KeywordSearchService>();
builder.Services.AddSingleton<EmbeddingService>();
builder.Services.AddSingleton<RerankerService>();
builder.Services.AddSingleton<LlmQueryUnderstandingService>();
builder.Services.AddSingleton<LlmRerankerService>();
builder.Services.AddSingleton<IntentChunkFilterService>();
builder.Services.AddSingleton<ConfidenceScoringService>();
builder.Services.AddSingleton<CitationService>();
builder.Services.AddSingleton<HallucinationDetectionService>();
builder.Services.AddSingleton<PdfKnowledgeService>();
builder.Services
    .AddSingleton<QdrantVectorStoreService>();
builder.Services
    .AddScoped<HybridSearchService>();
builder.Services.AddSingleton<MetadataFilterService>();
builder.Services.AddSingleton<RetrievalMetricsService>();
builder.Services.AddSingleton<MultiQueryService>();
builder.Services.AddSingleton<QueryExpansionService>();

var app = builder.Build();

using (var scope =
       app.Services.CreateScope())
{
    var pdfService =
        scope.ServiceProvider
            .GetRequiredService<
                PdfKnowledgeService>();

    var embeddingService =
        scope.ServiceProvider
            .GetRequiredService<
                EmbeddingService>();

    var qdrantStore =
        scope.ServiceProvider
            .GetRequiredService<
                QdrantVectorStoreService>();

    var policyExtractor =
        scope.ServiceProvider
            .GetRequiredService<
                PolicyExtractorService>();

    var categoryService =
        scope.ServiceProvider
            .GetRequiredService<
                CategoryService>();

    await VectorInitializationService
        .Initialize(
            pdfService,
            embeddingService,
            qdrantStore,
            policyExtractor,
            categoryService);
}



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
app.UseMiddleware<ExceptionMiddleware>();
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
