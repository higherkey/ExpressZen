using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace ExpressZen.Client.Services;

public class OpenAiAssistantService
{
    private readonly HttpClient _httpClient;
    public string ApiKey { get; set; } = string.Empty;

    public OpenAiAssistantService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<string> CallGptAsync(string systemInstruction, string userPrompt)
    {
        if (string.IsNullOrWhiteSpace(ApiKey)) return "Error: API Key not configured.";

        var requestBody = new
        {
            model = "gpt-4o",
            messages = new[]
            {
                new { role = "system", content = systemInstruction },
                new { role = "user", content = userPrompt }
            }
        };

        var requestContent = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
        
        var requestMessage = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/chat/completions");
        requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", ApiKey);
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
            var text = node?["choices"]?[0]?["message"]?["content"]?.ToString();
            
            return text ?? "Error parsing GPT response.";
        }
        catch (Exception ex)
        {
            return $"Exception: {ex.Message}";
        }
    }
}
