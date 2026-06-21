using MCPLearning.Plugins;
using MCPLearning.Services;
using MCPLearning.Tools;
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
        modelId: "qwen3",
        endpoint: new Uri("http://localhost:11434"));

    return kernelBuilder.Build();
});

//
// SQL Repository
//
builder.Services.AddScoped<CustomerRepository>();

//
// MCP Tools
//
builder.Services.AddScoped<IMcpTool, GetCustomerTool>();

builder.Services.AddScoped<McpToolRegistry>();

//
// Semantic Kernel Plugins
//
builder.Services.AddScoped<CustomerPlugin>();

//
// AI Copilot Service
//
builder.Services.AddScoped<CustomerCopilotService>();

builder.Services.AddScoped<DatePlugin>();
builder.Services.AddHttpClient<CurrencyPlugin>();
builder.Services.AddSingleton<
    ToolAuthorizationService>();

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