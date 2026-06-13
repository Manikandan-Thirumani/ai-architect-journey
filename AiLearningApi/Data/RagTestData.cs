namespace AiLearningApi.Data;

public static class RagTestData
{
    public static List<(string Question, string Expected)> Questions = new()
    {
        ("What is max loan for premium customers?", "50 lakhs"),
        ("What benefits do premium customers get?", "cashback up to 5%"),
        ("What is loan policy for standard customers?", "risk scoring and income")
    };
}