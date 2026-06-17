using AiLearningApi.Models;

namespace AiLearningApi.Data;

public static class RetrievalTestCases
{
    public static List<EvaluationTestCase>
        GetCases()
    {
        return new()
        {
            new()
            {
                Question =
                    "What benefits do premium customers receive?",

                ExpectedDocuments =
                {
                    "CreditCardPolicy.txt",
                    "InsurancePolicy.txt"
                }
            },

            new()
            {
                Question =
                    "What is the maximum loan amount?",

                ExpectedDocuments =
                {
                    "LoanPolicy.txt"
                }
            },

            new()
            {
                Question =
                    "What documents are needed for KYC?",

                ExpectedDocuments =
                {
                    "KYCPolicy.txt"
                }
            }
        };
    }
}