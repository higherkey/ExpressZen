using ExpressZen.Client.Models;

namespace ExpressZen.Client.Services;

public interface IRegexEngine
{
    string EngineName { get; }
    Task<RegexResult> EvaluateAsync(string pattern, string input, bool isGlobal = false);
}
