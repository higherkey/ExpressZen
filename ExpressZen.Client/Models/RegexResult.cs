namespace ExpressZen.Client.Models;

public record RegexResult
{
    public bool IsMatch { get; init; }
    public string GlobalMatchSummary { get; init; } = string.Empty;
    public string ErrorMessage { get; init; } = string.Empty;
    public TimeSpan ExecutionTime { get; init; }
}
