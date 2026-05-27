using System.Text.Json;
using AiLearningApi.Models;

namespace AiLearningApi.Services;

public class KnowledgeService
{
    private readonly List<KnowledgeItem> _knowledge;

    public KnowledgeService()
    {
        var json = File.ReadAllText(
            "Knowledge/architecture-knowledge.json");

        _knowledge =
            JsonSerializer.Deserialize<List<KnowledgeItem>>(json)
            ?? new();
    }

    public string SearchKnowledge(string question)
    {
        question = question.ToLower();

        foreach (var item in _knowledge)
        {
            if (question.Contains(item.topic.ToLower()))
            {
                return item.content;
            }
        }

        return "No relevant knowledge found.";
    }
}