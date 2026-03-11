using System.Text.Json;
using System.Text.Json.Nodes;

namespace ExpressZen.Client.Services;

public class GeminiAssistantService
{
    private readonly HttpClient _httpClient;
    public string ApiKey { get; set; } = string.Empty;

    public GeminiAssistantService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<string> CallGeminiAsync(string systemInstruction, string userPrompt, string model = "gemini-1.5-flash")
    {
        if (string.IsNullOrWhiteSpace(ApiKey)) return "Error: API Key not configured.";

        var requestBody = new
        {
            system_instruction = new { parts = new[] { new { text = systemInstruction } } },
            contents = new[]
            {
                new { parts = new[] { new { text = userPrompt } } }
            }
        };

        var requestContent = new StringContent(JsonSerializer.Serialize(requestBody), System.Text.Encoding.UTF8, "application/json");
        var url = $"https://generativelanguage.googleapis.com/v1beta/models/{model}:generateContent?key={ApiKey}";

        try
        {
            var response = await _httpClient.PostAsync(url, requestContent);
            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync();
                return $"API Error: {response.StatusCode}\n{errorBody}";
            }

            var json = await response.Content.ReadAsStringAsync();
            var node = JsonNode.Parse(json);
            var text = node?["candidates"]?[0]?["content"]?["parts"]?[0]?["text"]?.ToString();
            
            return text ?? "Error parsing Gemini response.";
        }
        catch (Exception ex)
        {
            return $"Exception: {ex.Message}";
        }
    }
}
