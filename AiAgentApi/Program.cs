using AiAgentApi.Agents;
using AiAgentApi.Plugins;
using AiAgentApi.Services;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.Ollama;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddOpenApi();

/*
 * Semantic Kernel
 */
builder.Services.AddSingleton<Kernel>(sp =>
{
    var kernelBuilder = Kernel.CreateBuilder();

    kernelBuilder.AddOllamaChatCompletion(
        modelId: "phi3",
        endpoint: new Uri("http://localhost:11434"));

    return kernelBuilder.Build();
});

/*
 * Plugins
 */
builder.Services.AddScoped<DateTimePlugin>();
builder.Services.AddScoped<RetrievalPlugin>();

/*
 * Agent Executor
 */
builder.Services.AddScoped<AssistantAgent>();
builder.Services.AddScoped<AgentExecutorService>();

/*
 * Knowledge Services
 */
builder.Services.AddHttpClient();

builder.Services.AddScoped<
    IKnowledgeService,
    KnowledgeService>();

builder.Services.AddScoped<
    IPlanGeneratorService,
    PlanGeneratorService>();

/*
 * Agents
 */
builder.Services.AddScoped<RetrievalAgent>();

builder.Services.AddScoped<DateAgent>();

builder.Services.AddScoped<CoordinatorAgent>();

builder.Services.AddScoped<ReviewerAgent>();

builder.Services.AddScoped<HumanApprovalAgent>();

/*
 * Week 11 Day 4
 * Persistent Memory Services
 */

builder.Services.AddHttpClient<
    IMemoryEmbeddingService,
    MemoryEmbeddingService>();

builder.Services.AddHttpClient<
    MemoryQdrantService>();

builder.Services.AddSingleton<IConversationMemoryService>(
    sp =>
    {
        var embedding =
            sp.GetRequiredService<
                IMemoryEmbeddingService>();

        var qdrant =
            sp.GetRequiredService<
                MemoryQdrantService>();

        return new ConversationMemoryService(
            embedding,
            qdrant);
    });
builder.Services.AddScoped<
    MemorySummarizerAgent>();
builder.Services.AddScoped<MemorySummarizerAgent>();
builder.Services.AddScoped<GroundingValidatorAgent>();


var app = builder.Build();

/*
 * Week 11 Day 4
 * Initialize Qdrant memory collection
 */
using (var scope = app.Services.CreateScope())
{
    var embedding =
        scope.ServiceProvider
            .GetRequiredService<
                IMemoryEmbeddingService>();

    var qdrant =
        scope.ServiceProvider
            .GetRequiredService<
                MemoryQdrantService>();

    var vector =
        await embedding.GenerateEmbeddingAsync(
            "test");

    await qdrant.InitializeAsync(
        vector.Count);
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