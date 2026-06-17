namespace AiLearningApi.Models;

public class BenchmarkResult
{
    public string Strategy { get; set; } = string.Empty;

    public double AverageRecall { get; set; }

    public double AveragePrecision { get; set; }

    public double AverageMrr { get; set; }
}