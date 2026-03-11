using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace ExpressZen.Client.Services;

public class AnthropicAssistantService
{
    private readonly HttpClient _httpClient;
    public string ApiKey { get; set; } = string.Empty;

    public AnthropicAssistantService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<string> CallClaudeAsync(string systemInstruction, string userPrompt)
    {
        if (string.IsNullOrWhiteSpace(ApiKey)) return "Error: API Key not configured.";

        var requestBody = new
        {
            model = "claude-3-5-sonnet-latest",
            max_tokens = 1024,
            system = systemInstruction,
            messages = new[]
            {
                new { role = "user", content = userPrompt }
            }
        };

        var requestContent = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
        
        // Anthropic requires specific headers
        var requestMessage = new HttpRequestMessage(HttpMethod.Post, "https://api.anthropic.com/v1/messages");
        requestMessage.Headers.Add("x-api-key", ApiKey);
        requestMessage.Headers.Add("anthropic-version", "2023-06-01");
        
        // Blazor WASM CORS Note: Anthropic's public API explicitly blocks standard browser CORS requests. 
        // We will add the antropic-dangerous-direct-browser-access header specifically for this BYOK POC mode.
        requestMessage.Headers.Add("anthropic-dangerous-direct-browser-access", "true");
        
        requestMessage.Content = requestContent;

        try
        {
            var response = await _httpClient.SendAsync(requestMessage);
            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync();
                return $"API Error: {response.StatusCode}\n{errorBody}";
            }

            var json = await response.Content.ReadAsStringAsync();
            var node = JsonNode.Parse(json);
            var text = node?["content"]?[0]?["text"]?.ToString();
            
            return text ?? "Error parsing Claude response.";
        }
        catch (Exception ex)
        {
            return $"Exception: {ex.Message}";
        }
    }
}
