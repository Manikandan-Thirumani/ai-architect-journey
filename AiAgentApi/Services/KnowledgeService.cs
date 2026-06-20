public class KnowledgeService : IKnowledgeService
{
    public async Task<string> SearchAsync(string question)
    {
        var text = await File.ReadAllTextAsync(
            "KnowledgeBase/BankingPolicies.txt");

        return text;
    }
}