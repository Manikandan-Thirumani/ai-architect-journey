using Microsoft.SemanticKernel;
using ShoppingAgentClient.MCP;
using ShoppingAgentClient.Services;

var builder =
    WebApplication.CreateBuilder(args);

/*
 * Controllers
 */
builder.Services
    .AddControllers();

builder.Services
    .AddOpenApi();

/*
 * MCP Client
 */
builder.Services
    .AddHttpClient<
        ShoppingMcpClient>(
        client =>
        {
            client.BaseAddress =
                new Uri(
                    "https://localhost:7291");
        });

/*
 * Semantic Kernel
 */
builder.Services
    .AddSingleton<Kernel>(sp =>
    {
        var kernelBuilder =
            Kernel.CreateBuilder();

        kernelBuilder
            .AddOllamaChatCompletion(
                modelId: "qwen2.5:7b",
                endpoint:
                    new Uri(
                        "http://localhost:11434"));

        return kernelBuilder.Build();
    });

/*
 * Shopping Agent
 */
builder.Services
    .AddSingleton<
        ShoppingAgent>();

var app =
    builder.Build();

/*
 * HTTP pipeline
 */
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();