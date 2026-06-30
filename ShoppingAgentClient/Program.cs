using Microsoft.SemanticKernel;
using ShoppingAgentClient.MCP;
using ShoppingAgentClient.Services;

var builder =
    WebApplication
        .CreateBuilder(args);

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
var kernelBuilder =
    Kernel.CreateBuilder();

kernelBuilder
    .AddOllamaChatCompletion(
        modelId: "phi3",
        endpoint:
            new Uri(
                "http://localhost:11434"));

var kernel =
    kernelBuilder
        .Build();

builder.Services
    .AddSingleton(
        kernel);



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