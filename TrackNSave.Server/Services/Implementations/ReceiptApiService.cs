using System.Text.Json;
using System.Text;
using TrackNSave.Server.Services.Interfaces;

namespace TrackNSave.Server.Services.Implementations
{
    public class ReceiptApiService : IReceiptApiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiUrl = "https://proverkacheka.com/api/v1/check/get";
        private readonly string _apiToken;

        public ReceiptApiService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiToken = configuration["API_TOKEN"];
        }

        public async Task<JsonElement?> FetchReceiptDataAsync(string qrRaw)
        {
            var apiRequest = new { qrraw = qrRaw, token = _apiToken };
            var requestContent = new StringContent(JsonSerializer.Serialize(apiRequest), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(_apiUrl, requestContent);
            if (!response.IsSuccessStatusCode) return null;

            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<JsonElement>(responseContent);
        }
    }
}