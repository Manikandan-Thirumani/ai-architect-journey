using MCPLearning.Models;

namespace MCPLearning.Services;

public class McpRateLimitService
{
    private readonly Dictionary<string, RateLimitEntry>
        _entries = new();

    public bool IsAllowed(
        string role,
        out string message)
    {
        int limit =
            role.ToLower() switch
            {
                "customer" => 5,
                "employee" => 20,
                "manager" => 50,
                _ => 3
            };

        var key = role.ToLower();

        if (!_entries.ContainsKey(key))
        {
            _entries[key] =
                new RateLimitEntry
                {
                    Count = 0,
                    WindowStart = DateTime.UtcNow
                };
        }

        var entry =
            _entries[key];

        /*
         * Reset every minute.
         */

        if (DateTime.UtcNow >
            entry.WindowStart.AddMinutes(1))
        {
            entry.Count = 0;
            entry.WindowStart =
                DateTime.UtcNow;
        }

        if (entry.Count >= limit)
        {
            message =
                $"Rate limit exceeded. " +
                $"Maximum {limit} requests " +
                $"per minute allowed.";

            return false;
        }

        entry.Count++;

        message = "";

        return true;
    }
}