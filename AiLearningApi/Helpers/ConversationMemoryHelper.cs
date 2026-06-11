namespace AiLearningApi.Helpers
{
    public static class ConversationMemoryHelper
    {
        public static bool NeedsRewrite(
            string question)
        {
            var words =
                question.Split(
                    ' ',
                    StringSplitOptions
                        .RemoveEmptyEntries);

            return words.Length <= 5;
        }
    }
}
