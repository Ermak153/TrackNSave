using System.Text.Json;
using TrackNSave.Server.Models;
using TrackNSave.Server.Services.Interfaces;

namespace TrackNSave.Server.Services.Implementations
{
    public class ReceiptParserService : IReceiptParserService
    {
        public FiscalData ExtractFiscalData(JsonElement rawData)
        {
            if (!rawData.TryGetProperty("data", out var data) ||
                !data.TryGetProperty("json", out var json))
            {
                return new FiscalData();
            }

            return new FiscalData
            {
                FiscalSign = ExtractString(json, "fiscalSign"),
                FiscalDriveNumber = ExtractString(json, "fiscalDriveNumber"),
                FiscalDocumentNumber = ExtractString(json, "fiscalDocumentNumber")
            };
        }

        public FormattedReceipt FormatReceipt(JsonElement rawData)
        {
            if (!rawData.TryGetProperty("data", out var data) || !data.TryGetProperty("json", out var json))
            {
                return null;
            }

            return new FormattedReceipt
            {
                User = json.TryGetProperty("user", out var user) ? user.GetString() ?? "Неизвестный продавец" : "Неизвестный продавец",
                TotalSum = json.TryGetProperty("totalSum", out var totalSum) ? totalSum.GetInt32() : 0,
                DateTime = json.TryGetProperty("dateTime", out var dateTime) ? dateTime.GetString() ?? "Неизвестное время" : "Неизвестное время",
                RetailPlace = json.TryGetProperty("retailPlace", out var retailPlace) ? retailPlace.GetString() ?? "Неизвестное место" : "Неизвестное место",
                Items = json.TryGetProperty("items", out var items)
                    ? items.EnumerateArray()
                        .Select(item => new ReceiptItem
                        {
                            Name = item.TryGetProperty("name", out var name) ? name.GetString() ?? "Неизвестный товар" : "Неизвестный товар",
                            Price = item.TryGetProperty("price", out var price) ? price.GetInt32() : 0,
                            Quantity = item.TryGetProperty("quantity", out var quantity) ? quantity.GetInt32() : 0,
                            Sum = item.TryGetProperty("sum", out var sum) ? sum.GetInt32() : 0
                        })
                        .ToList()
                    : new List<ReceiptItem>()
            };
        }

        private string? ExtractString(JsonElement json, string propertyName)
        {
            if (json.TryGetProperty(propertyName, out var element))
            {
                return element.ValueKind == JsonValueKind.String
                    ? element.GetString()
                    : element.GetRawText();
            }
            return "";
        }
    }
}