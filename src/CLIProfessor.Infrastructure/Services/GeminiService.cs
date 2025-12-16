using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using CLIProfessor.Domain.Interfaces;
using Polly;
using Polly.Retry;

namespace CLIProfessor.Infrastructure.Services;

public class GeminiService : IGeminiService
{
    private readonly string _apiKey;
    private readonly HttpClient _httpClient;
    private readonly AsyncRetryPolicy<HttpResponseMessage> _retryPolicy;

    public GeminiService(IConfiguration configuration)
    {
        _apiKey = configuration["Gemini:ApiKey"] ?? throw new ArgumentNullException("Gemini:ApiKey");
        _httpClient = new HttpClient(); // In a real app, inject IHttpClientFactory

        // Retry on TooManyRequests (429) or ServiceUnavailable (503)
        _retryPolicy = Policy
            .HandleResult<HttpResponseMessage>(r => r.StatusCode == System.Net.HttpStatusCode.TooManyRequests || 
                                                    r.StatusCode == System.Net.HttpStatusCode.ServiceUnavailable)
            .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
    }

    public async Task<string> GenerateContentAsync(string prompt)
    {
        var url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent?key={_apiKey}";

        var requestBody = new
        {
            contents = new[]
            {
                new { parts = new[] { new { text = prompt } } }
            }
        };

        var json = JsonSerializer.Serialize(requestBody);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _retryPolicy.ExecuteAsync(() => _httpClient.PostAsync(url, content));
        if (!response.IsSuccessStatusCode)
        {
             var error = await response.Content.ReadAsStringAsync();
             Console.WriteLine($"Gemini Content Error: {response.StatusCode} - {error}");
        }
        response.EnsureSuccessStatusCode();

        var responseJson = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(responseJson);
        
        // Navigate: candidates[0].content.parts[0].text
        // Note: Safety checks omitted for brevity, but highly recommended
        try 
        {
            return doc.RootElement
                .GetProperty("candidates")[0]
                .GetProperty("content")
                .GetProperty("parts")[0]
                .GetProperty("text")
                .GetString() ?? "No response text found.";
        }
        catch
        {
            return "Error parsing Gemini response.";
        }
    }

    public async Task<float[]> GenerateEmbeddingAsync(string text)
    {
        var url = $"https://generativelanguage.googleapis.com/v1beta/models/text-embedding-004:embedContent?key={_apiKey}";

        var requestBody = new
        {
            content = new { parts = new[] { new { text = text } } },
            taskType = "RETRIEVAL_QUERY"
        };

        var json = JsonSerializer.Serialize(requestBody);
        Console.WriteLine($"Gemini Embedding Request: {json}");
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _retryPolicy.ExecuteAsync(() => _httpClient.PostAsync(url, content));
        if (!response.IsSuccessStatusCode)
        {
             var error = await response.Content.ReadAsStringAsync();
             Console.WriteLine($"Gemini Embedding Error: {response.StatusCode} - {error}");
             throw new HttpRequestException($"Gemini Embedding Error: {response.StatusCode} - {error}");
        }
        response.EnsureSuccessStatusCode();

        var responseJson = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(responseJson);

        try
        {
            var values = doc.RootElement
                .GetProperty("embedding")
                .GetProperty("values");

            var floatArray = new float[values.GetArrayLength()];
            int i = 0;
            foreach (var val in values.EnumerateArray())
            {
                floatArray[i++] = val.GetSingle();
            }
            return floatArray;
        }
        catch
        {
            return Array.Empty<float>();
        }
    }
}
