using AiLearningApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddScoped<OpenAiService>();
builder.Services.AddHttpClient<OllamaService>(client =>
{
    client.Timeout = TimeSpan.FromMinutes(10);
});
builder.Services.AddScoped<SemanticKernelService>();
builder.Services.AddSingleton<KnowledgeService>();

builder.Services.AddSingleton<RagService>();
builder.Services.AddSingleton<BankKnowledgeService>();

builder.Services.AddSingleton<BankingRagService>();
builder.Services
    .AddSingleton<PdfKnowledgeService>();

builder.Services
    .AddSingleton<PdfChunkingService>();

builder.Services
    .AddSingleton<ChunkRetrievalService>();


builder.Services
    .AddSingleton<EmbeddingService>();

builder.Services
    .AddSingleton<VectorStoreService>();
builder.Services.AddSingleton<SemanticRetrievalService>();
builder.Services
    .AddSingleton<RerankerService>();
builder.Services
    .AddSingleton<CategoryService>();
builder.Services
    .AddSingleton<PolicyExtractorService>();
builder.Services
    .AddSingleton<QueryUnderstandingService>();
builder.Services
    .AddSingleton<
        LlmQueryUnderstandingService>();
builder.Services
    .AddSingleton<
        LlmRerankerService>();
builder.Services.AddSingleton<
    IntentChunkFilterService>();
builder.Services
    .AddSingleton<
        ConfidenceScoringService>();
builder.Services
    .AddSingleton<
        CitationService>();
builder.Services
    .AddSingleton<
        HallucinationDetectionService>();
builder.Services
    .AddSingleton<QueryRewriteService>();
builder.Services
    .AddSingleton<
        ConversationMemoryService>();

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

    var vectorStore =
        scope.ServiceProvider
            .GetRequiredService<
                VectorStoreService>();

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
            vectorStore,
            policyExtractor,
            categoryService);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
