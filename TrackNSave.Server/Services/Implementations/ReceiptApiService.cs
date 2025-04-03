using System.Text.Json;
using System.Text;
using TrackNSave.Server.Services.Interfaces;

namespace TrackNSave.Server.Services.Implementations
{
    public class ReceiptApiException : Exception
    {
        public int StatusCode { get; }
        public ReceiptApiException(int statusCode, string message) : base(message) { StatusCode = statusCode; }
    }

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

        public async Task<JsonElement?> FetchReceiptDataAsync(string receiptRaw)
        {
            var apiRequest = new { qrraw = receiptRaw, token = _apiToken };
            var requestContent = new StringContent(JsonSerializer.Serialize(apiRequest), Encoding.UTF8, "application/json");

            HttpResponseMessage response;
            try
            {
                response = await _httpClient.PostAsync(_apiUrl, requestContent);
            }
            catch (HttpRequestException)
            {
                throw new ReceiptApiException(502, "Network error when contacting the receipt API");
            }

            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new ReceiptApiException(502, "Receipt API returned error");
            }

            try
            {
                var jsonResponse = JsonSerializer.Deserialize<JsonElement>(responseContent);
                int code = jsonResponse.GetProperty("code").GetInt32();

                switch (code)
                {
                    case 1:
                        return jsonResponse;
                    case 2:
                        throw new ReceiptApiException(202, "The receipt data is not available yet, or the receipt is incorrect");
                    case 3:
                        throw new ReceiptApiException(429, "Exceeded number of requests for this receipt");
                    case 4:
                        throw new ReceiptApiException(429, "Too many requests, wait before retrying");
                    case 5:
                        throw new ReceiptApiException(503, "No receipt data available at the moment, retry later.");
                    case 0:
                        throw new ReceiptApiException(400, "Invalid receipt");
                    default:
                        throw new ReceiptApiException(500, "Unknown response code from API");
                }
            }
            catch (JsonException)
            {
                throw new JsonException("Failed to deserialize receipt API response");
            }
        }
    }
}