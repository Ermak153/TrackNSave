using System.Text.Json;

namespace TrackNSave.Server.Services.Interfaces
{
    public interface IReceiptApiService
    {
        Task<JsonElement?> FetchReceiptDataAsync(string receiptRaw);
        Task<List<string>> FetchProductCategoryAsync(List<string> productNames);
    }
}