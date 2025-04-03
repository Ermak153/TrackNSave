using System.Text.Json;

namespace TrackNSave.Server.Services.Interfaces
{
    public interface IReceiptPdfService
    {
        Task<byte[]> GenerateReceiptPdfAsync(JsonDocument rawData);
    }
}
