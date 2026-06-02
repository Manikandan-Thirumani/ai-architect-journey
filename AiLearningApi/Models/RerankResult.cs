using AiLearningApi.Models;

namespace AiLearningApi.Models;

public class RerankResult
{
    public VectorDocument Document
    {
        get;
        set;
    } = default!;

    public double VectorScore
    {
        get;
        set;
    }

    public int RerankScore
    {
        get;
        set;
    }
}