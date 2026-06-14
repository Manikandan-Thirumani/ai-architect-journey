namespace AiLearningApi.Services;

public class MetadataFilterService
{
    public string DetectCategory(
        string query)
    {
        query =
            query.ToLower();

        if (query.Contains("loan"))
            return "Loan";

        if (query.Contains("insurance"))
            return "Insurance";

        if (query.Contains("credit"))
            return "Credit Card";

        return "General";
    }
}