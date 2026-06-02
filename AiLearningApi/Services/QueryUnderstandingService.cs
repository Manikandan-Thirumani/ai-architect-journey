using AiLearningApi.Models;

namespace AiLearningApi.Services;

public class QueryUnderstandingService
{
    public QueryIntent Analyze(
        string question)
    {
        question =
            question.ToLower();

        var result =
            new QueryIntent();

        // Category

        if (question.Contains("loan"))
        {
            result.Category =
                "Loan";
        }
        else if (question.Contains("credit"))
        {
            result.Category =
                "CreditCard";
        }
        else if (question.Contains("deposit"))
        {
            result.Category =
                "Deposit";
        }
        else if (question.Contains("fraud"))
        {
            result.Category =
                "Fraud";
        }

        // Intent

        if (question.Contains("amount")
            || question.Contains("max")
            || question.Contains("maximum"))
        {
            result.Intent =
                "MaximumAmount";
        }
        else if (question.Contains("interest"))
        {
            result.Intent =
                "InterestRate";
        }
        else if (question.Contains("age"))
        {
            result.Intent =
                "AgeEligibility";
        }

        return result;
    }
}