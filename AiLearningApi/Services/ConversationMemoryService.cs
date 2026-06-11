namespace AiLearningApi.Services;

public class ConversationMemoryService
{
    private readonly List<string>
        _history = [];

    public void AddMessage(
        string message)
    {
        _history.Add(message);

        if (_history.Count > 10)
        {
            _history.RemoveAt(0);
        }
    }

    public string GetContext()
    {
        return string.Join(
            Environment.NewLine,
            _history);
    }
}