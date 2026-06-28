using EnterpriseSqlCopilot.MCP;
using EnterpriseSqlCopilot.Services;
using Microsoft.SemanticKernel;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddOpenApi();

//
// Semantic Kernel + Ollama
//
builder.Services.AddSingleton<Kernel>(sp =>
{
    var kernelBuilder = Kernel.CreateBuilder();

    kernelBuilder.AddOllamaChatCompletion(
        modelId: "phi3",
        endpoint: new Uri("http://localhost:11434"));

    return kernelBuilder.Build();
});

//
// Legacy SQL services
// (Keep temporarily until full MCP migration)
//
builder.Services.AddScoped<DatabaseService>();

builder.Services.AddScoped<SchemaRepository>();

builder.Services.AddScoped<SchemaDiscoveryService>();

builder.Services.AddScoped<SqlExecutionService>();

builder.Services.AddScoped<SqlGuardrailService>();

builder.Services.AddScoped<SqlGenerationService>();
builder.Services.AddScoped<
    SqlCopilotOrchestrator>();
builder.Services
    .AddScoped<
        McpPlannerService>();
builder.Services.AddScoped<
    SqlRepairService>();
//
// MCP Client
//
builder.Services.AddHttpClient<McpClientService>(client =>
{
    client.BaseAddress =
        new Uri("https://localhost:7277");
});

var app = builder.Build();

//
// Configure the HTTP request pipeline.
//
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();