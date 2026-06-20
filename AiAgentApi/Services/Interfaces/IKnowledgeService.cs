public interface IKnowledgeService
{
    Task<string> SearchAsync(string question);
}