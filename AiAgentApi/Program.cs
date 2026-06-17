using AiAgentApi.Agents;
using AiAgentApi.Plugins;
using AiAgentApi.Services;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.Ollama;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddSingleton<Kernel>(sp =>
{
    var kernelBuilder = Kernel.CreateBuilder();

    kernelBuilder.AddOllamaChatCompletion(
        modelId: "phi3",
        endpoint: new Uri("http://localhost:11434"));

    return kernelBuilder.Build();
});
builder.Services.AddScoped<DateTimePlugin>();
builder.Services.AddScoped<
    AssistantAgent>();
builder.Services.AddScoped<
    AgentExecutorService>();

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
