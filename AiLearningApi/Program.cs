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
    .AddSingleton<BankingRagService>();
builder.Services
    .AddSingleton<PdfChunkingService>();

builder.Services
    .AddSingleton<ChunkRetrievalService>();

builder.Services
    .AddSingleton<BankingRagService>();
builder.Services
    .AddSingleton<EmbeddingService>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
