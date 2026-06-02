namespace AiLearningApi.Services;

public class CategoryService
{
    public string GetCategory(
        string policyName)
    {
        policyName =
            policyName.ToLower();

        if (policyName.Contains("loan"))
            return "Loan";

        if (policyName.Contains("credit"))
            return "CreditCard";

        if (policyName.Contains("deposit"))
            return "Deposit";

        if (policyName.Contains("fraud"))
            return "Fraud";

        if (policyName.Contains("digital"))
            return "DigitalBanking";

        return "General";
    }
}