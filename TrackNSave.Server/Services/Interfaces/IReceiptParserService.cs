using System.Text.Json;
using TrackNSave.Server.Models;
using TrackNSave.Server.Models.DTOs;

namespace TrackNSave.Server.Services.Interfaces
{
    public interface IReceiptParserService
    {
        QrCodeData ExtractQrCodeData(JsonElement rawData);
        FiscalData ExtractFiscalData(JsonElement rawData);
        Task<FormattedReceipt> FormatReceiptAsync(JsonElement rawData);
    }
}