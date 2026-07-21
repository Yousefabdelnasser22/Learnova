using Learnova.Infrastructure.Configuration;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using System.Text.Json;

namespace Learnova.Infrastructure.Search
{
    public class EmbeddingService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string _model;

        public EmbeddingService(IOptions<GeminiSettings> options, HttpClient httpClient)
        {
            _apiKey = options.Value.ApiKey;
            _model = options.Value.EmbeddingModel;
            _httpClient = httpClient;
        }

        public async Task<float[]> GenerateEmbeddingAsync(string text, CancellationToken cancellationToken = default)
        {
            var url = $"https://generativelanguage.googleapis.com/v1beta/models/{_model}:embedContent?key={_apiKey}";

            var body = new
            {
                model = $"models/{_model}",
                content = new
                {
                    parts = new[] { new { text } }
                }
            };

            var response = await _httpClient.PostAsJsonAsync(url, body, cancellationToken);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<JsonElement>(cancellationToken: cancellationToken);
            var values = result
                .GetProperty("embedding")
                .GetProperty("values")
                .EnumerateArray()
                .Select(v => v.GetSingle())
                .ToArray();

            return values;
        }
    }
}
