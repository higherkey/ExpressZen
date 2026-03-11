using System.Diagnostics;
using System.Text.RegularExpressions;
using ExpressZen.Client.Models;

namespace ExpressZen.Client.Services;

public class DotNetRegexEngine : IRegexEngine
{
    public string EngineName => ".NET (C#)";

    public Task<RegexResult> EvaluateAsync(string pattern, string input, bool isGlobal = false)
    {
        var sw = Stopwatch.StartNew();
        
        try
        {
            // ReDoS Prevention: We enforce NonBacktracking or a strict Timeout.
            // Using a 500ms timeout for standard evaluation. 
            var options = RegexOptions.None;
            var timeout = TimeSpan.FromMilliseconds(500);

            if (string.IsNullOrEmpty(pattern) || input == null)
            {
                return Task.FromResult(new RegexResult { IsMatch = false, ExecutionTime = sw.Elapsed });
            }

            if (isGlobal)
            {
                var matches = Regex.Matches(input, pattern, options, timeout);
                sw.Stop();
                return Task.FromResult(new RegexResult 
                { 
                    IsMatch = matches.Count > 0,
                    GlobalMatchSummary = $"Found {matches.Count} matches.",
                    ExecutionTime = sw.Elapsed 
                });
            }
            else
            {
                var isMatch = Regex.IsMatch(input, pattern, options, timeout);
                sw.Stop();
                return Task.FromResult(new RegexResult 
                { 
                    IsMatch = isMatch,
                    ExecutionTime = sw.Elapsed 
                });
            }
        }
        catch (RegexMatchTimeoutException)
        {
            sw.Stop();
            return Task.FromResult(new RegexResult 
            { 
                IsMatch = false, 
                ErrorMessage = "Catastrophic Backtracking Detected! Execution halted after 500ms.",
                ExecutionTime = sw.Elapsed 
            });
        }
        catch (ArgumentException ex)
        {
            sw.Stop();
            return Task.FromResult(new RegexResult 
            { 
                IsMatch = false, 
                ErrorMessage = ex.Message,
                ExecutionTime = sw.Elapsed 
            });
        }
        catch (Exception ex)
        {
            sw.Stop();
            return Task.FromResult(new RegexResult 
            { 
                IsMatch = false, 
                ErrorMessage = $"Engine Error: {ex.Message}",
                ExecutionTime = sw.Elapsed 
            });
        }
    }
}
