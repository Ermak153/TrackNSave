using System.Text.Json;
using TrackNSave.Server.Models;

namespace TrackNSave.Server.Services.Interfaces
{
    public interface IReceiptParserService
    {
        QrCodeData ExtractQrCodeData(JsonElement rawData);
        FiscalData ExtractFiscalData(JsonElement rawData);
        Task<FormattedReceipt> FormatReceiptAsync(JsonElement rawData);
    }
}