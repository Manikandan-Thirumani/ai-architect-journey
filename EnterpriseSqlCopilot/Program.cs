using EnterpriseSqlCopilot.Services;
using Microsoft.SemanticKernel;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddScoped<DatabaseService>();

builder.Services.AddScoped<SchemaRepository>();

builder.Services.AddScoped<
    SchemaDiscoveryService>();
builder.Services.AddScoped<SqlGenerationService>();
builder.Services.AddScoped<
    SqlGuardrailService>();

builder.Services.AddScoped<
    SqlExecutionService>();

builder.Services.AddScoped<
    CopilotService>();
builder.Services.AddSingleton<Kernel>(sp =>
{
    var kernelBuilder =
        Kernel.CreateBuilder();

    kernelBuilder.AddOllamaChatCompletion(
        modelId: "phi3",
        endpoint: new Uri("http://localhost:11434"));

    return kernelBuilder.Build();
});
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
