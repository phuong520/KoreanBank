// Đặt trong một file riêng như Services/GeminiApiService.cs
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using KEB.Application.DTOs.GeminiDto;
using KEB.Application.Services;

public class GeminiApiService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly string _modelName;
    private readonly string _baseApiUrl;

    public GeminiApiService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _apiKey = "AIzaSyATIkm90Fjt9UY5lmKf1ekpz25VQHgbXGU";
    }

    public async Task<string> GenerateContentAsync(string prompt)
    {
        var url = $"{_baseApiUrl}{_modelName}:generateContent?key={_apiKey}";

        var requestBody = new GeminiRequestBody
        {
            Contents = new List<Content>
            {
                new Content
                {
                    Parts = new List<Part> { new Part { Text = prompt } }
                }
            },
            GenerationConfig = new GenerationConfig
            {
                Temperature = 0.2, // Giá trị thấp hơn để có kết quả nhất quán hơn
                MaxOutputTokens = 150, // Đủ lớn để Gemini trả lời "CÓ (x, y)" hoặc "KHÔNG"
                TopK = 40,
                TopP = 0.95
            }
        };

        var json = JsonConvert.SerializeObject(requestBody);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        try
        {
            var response = await _httpClient.PostAsync(url, content);
            response.EnsureSuccessStatusCode(); 

            var responseString = await response.Content.ReadAsStringAsync();
            var geminiResponse = JsonConvert.DeserializeObject<GeminiResponse>(responseString);

            if (geminiResponse?.Candidates != null && geminiResponse.Candidates.Any())
            {
                return geminiResponse.Candidates[0].Content.Parts[0].Text;
            }
            return null;
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"[GeminiApiService] Lỗi HTTP khi gọi Gemini API: {ex.StatusCode} - {ex.Message}");
            throw; // Re-throw để được xử lý ở lớp gọi
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[GeminiApiService] Lỗi không mong muốn: {ex.Message}");
            throw;
        }
    }
}