using System.Text.RegularExpressions;

namespace AiLearningApi.Services;

public class PolicyExtractorService
{
    public string ExtractPolicyName(
        string content)
    {
        var match =
            Regex.Match(
                content,
                @"Chapter\s+\d+\s*-\s*(.+?Policy)");

        if (match.Success)
        {
            return match.Groups[1]
                .Value
                .Trim();
        }

        return "Unknown";
    }
}