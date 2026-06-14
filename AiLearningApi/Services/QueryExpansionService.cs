namespace AiLearningApi.Services;

public class QueryExpansionService
{
    public string Expand(
        string query)
    {
        query =
            query.ToLower();

        if (query.Contains(
            "loan eligibility"))
        {
            return query +
                   " income validation risk assessment repayment";
        }

        if (query.Contains(
            "credit card"))
        {
            return query +
                   " cashback rewards lounge access";
        }

        return query;
    }
}