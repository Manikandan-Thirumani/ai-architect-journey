namespace AiLearningApi.Services;

public class PolicyExtractorService
{
    public string ExtractPolicyName(
        string content)
    {
        var lines =
            content.Split(
                '\n',
                StringSplitOptions.RemoveEmptyEntries);

        foreach (var line in lines)
        {
            var text =
                line.Trim();

            if (text.StartsWith(
                    "Chapter",
                    StringComparison.OrdinalIgnoreCase))
            {
                var parts =
                    text.Split('-');

                if (parts.Length > 1)
                {
                    return parts[1].Trim();
                }
            }
        }

        return "Unknown";
    }
}