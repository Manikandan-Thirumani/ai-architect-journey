using AiLearningApi.Models;

namespace AiLearningApi.Services;

public class FakeRetriever
{
    public List<RetrievedChunk> Retrieve(string query)
    {
        return new List<RetrievedChunk>
        {
            new RetrievedChunk
            {
                Content = "Premium customers can get loans up to 50 lakhs subject to credit and income verification.",
                SourceDocument = "CreditCardPolicy.txt",
                PolicyName = "Loan Policy",
                Score = 0.92
            },
            new RetrievedChunk
            {
                Content = "Premium customers receive cashback up to 5% on eligible transactions.",
                SourceDocument = "BenefitsPolicy.txt",
                PolicyName = "Credit Card Benefits",
                Score = 0.87
            },
            new RetrievedChunk
            {
                Content = "Standard customers have loan limits based on risk scoring and income.",
                SourceDocument = "CreditCardPolicy.txt",
                PolicyName = "Loan Policy",
                Score = 0.70
            }
        };
    }
}