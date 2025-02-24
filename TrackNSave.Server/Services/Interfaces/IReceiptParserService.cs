using System.Text.Json;
using TrackNSave.Server.Models;

namespace TrackNSave.Server.Services.Interfaces
{
    public interface IReceiptParserService
    {
        FiscalData ExtractFiscalData(JsonElement rawData);
        FormattedReceipt FormatReceipt(JsonElement rawData);
    }
}