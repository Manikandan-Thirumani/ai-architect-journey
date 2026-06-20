using Microsoft.SemanticKernel;

public class RetrievalPlugin
{
    private readonly IKnowledgeService _knowledge;

    public RetrievalPlugin(
        IKnowledgeService knowledge)
    {
        _knowledge = knowledge;
    }

    [KernelFunction]
    public async Task<string> SearchKnowledgeBase(
        string question)
    {
        return await _knowledge.SearchAsync(question);
    }
}