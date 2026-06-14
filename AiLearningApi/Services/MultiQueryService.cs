namespace AiLearningApi.Services;

public class MultiQueryService
{
    private readonly
        QueryExpansionService
            _expander;

    public MultiQueryService(
        QueryExpansionService expander)
    {
        _expander = expander;
    }

    public List<string>
        GenerateQueries(
            string question)
    {
        return
        [
            question,
            _expander.Expand(
                question)
        ];
    }
}