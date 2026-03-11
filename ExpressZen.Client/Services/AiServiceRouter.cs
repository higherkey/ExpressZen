using System.Text.Json;

namespace ExpressZen.Client.Services;

public class AiServiceRouter : IAiAssistantService
{
    private readonly GeminiAssistantService _gemini;
    private readonly AnthropicAssistantService _anthropic;
    private readonly OpenAiAssistantService _openai;

    public AiProvider ActiveProvider { get; private set; } = AiProvider.GoogleGemini;

    public bool IsConfigured => ActiveProvider switch
    {
        AiProvider.GoogleGemini => !string.IsNullOrWhiteSpace(_gemini.ApiKey),
        AiProvider.Anthropic => !string.IsNullOrWhiteSpace(_anthropic.ApiKey),
        AiProvider.OpenAi => !string.IsNullOrWhiteSpace(_openai.ApiKey),
        _ => false
    };

    public AiServiceRouter(
        GeminiAssistantService gemini,
        AnthropicAssistantService anthropic,
        OpenAiAssistantService openai)
    {
        _gemini = gemini;
        _anthropic = anthropic;
        _openai = openai;
    }

    public void ConfigureApiKey(AiProvider provider, string apiKey)
    {
        ActiveProvider = provider;
        switch (provider)
        {
            case AiProvider.GoogleGemini:
                _gemini.ApiKey = apiKey;
                break;
            case AiProvider.Anthropic:
                _anthropic.ApiKey = apiKey;
                break;
            case AiProvider.OpenAi:
                _openai.ApiKey = apiKey;
                break;
        }
    }

    private async Task<string> CallActiveProviderAsync(string systemInstruction, string userPrompt)
    {
        return ActiveProvider switch
        {
            AiProvider.GoogleGemini => await _gemini.CallGeminiAsync(systemInstruction, userPrompt),
            AiProvider.Anthropic => await _anthropic.CallClaudeAsync(systemInstruction, userPrompt),
            AiProvider.OpenAi => await _openai.CallGptAsync(systemInstruction, userPrompt),
            _ => "Error: Unknown provider selected."
        };
    }

    public async Task<string> BuildRegexAsync(string plainEnglishDescription)
    {
        var sys = "You are an expert regex builder. Output ONLY the raw regular expression pattern. Do not include markdown formatting or backticks around the pattern. Provide absolutely no explanation or conversational filler.";
        return await CallActiveProviderAsync(sys, $"Build a .NET regex for: {plainEnglishDescription}");
    }

    public async Task<string> ExplainRegexAsync(string pattern)
    {
        var sys = "You are a friendly, neuro-inclusive regex teacher. Explain this regex in simple, clear terms, breaking down each component. Format with markdown bullet points for readability. Keep it concise.";
        
        // Custom logic: Gemini requires a specific advanced model override for Explainer depth. 
        if (ActiveProvider == AiProvider.GoogleGemini)
        {
            return await _gemini.CallGeminiAsync(sys, $"Explain this .NET regex: {pattern}", "gemini-1.5-pro");
        }
        
        return await CallActiveProviderAsync(sys, $"Explain this .NET regex: {pattern}");
    }

    public async Task<string> TroubleshootRegexAsync(string pattern, string failingString)
    {
        var sys = "You are an expert regex troubleshooter. Review the pattern and the failing string, suggest exactly how to fix the pattern, and explain why. Keep it concise.";
        return await CallActiveProviderAsync(sys, $"Pattern: {pattern}\nFailing string: {failingString}");
    }

    public async Task<IEnumerable<string>> GenerateTestDataAsync(string pattern, bool shouldMatch)
    {
        var condition = shouldMatch ? "SHOULD match" : "SHOULD NOT match";
        var sys = "You are a test data generator for TDD regex environments. Given a regex pattern, generate a JSON array of 3 distinct, realistic strings that " + condition + " the pattern. ONLY output the raw JSON array (e.g. [\"str1\", \"str2\", \"str3\"]), no markdown formatting, no backticks, no code blocks.";
        var json = await CallActiveProviderAsync(sys, $"Pattern: {pattern}");
        
        try
        {
            json = json.Replace("```json", "").Replace("```", "").Trim();
            return JsonSerializer.Deserialize<List<string>>(json) ?? new List<string>();
        }
        catch
        {
            return new List<string> { "Error generating data.", "Please try again.", json };
        }
    }
}
