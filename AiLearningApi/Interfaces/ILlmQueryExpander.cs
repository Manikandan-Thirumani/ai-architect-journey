public interface ILlmQueryExpander
{
    Task<List<string>> ExpandAsync(string query);
}