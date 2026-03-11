namespace ExpressZen.Client.Services;

public enum AiProvider
{
    GoogleGemini,
    OpenAi,
    Anthropic
}

public interface IAiAssistantService
{
    bool IsConfigured { get; }
    AiProvider ActiveProvider { get; }
    void ConfigureApiKey(AiProvider provider, string apiKey);
    
    Task<string> BuildRegexAsync(string plainEnglishDescription);
    Task<string> ExplainRegexAsync(string pattern);
    Task<string> TroubleshootRegexAsync(string pattern, string failingString);
    Task<IEnumerable<string>> GenerateTestDataAsync(string pattern, bool shouldMatch);
}
