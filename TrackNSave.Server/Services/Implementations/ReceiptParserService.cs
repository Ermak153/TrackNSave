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

        public QrCodeData ExtractQrCodeData(JsonElement rawData)
        {
            if (!rawData.TryGetProperty("request", out var request))
            {
                return new QrCodeData();
            }

            var qrRaw = request.TryGetProperty("qrraw", out var qrRawProperty)
                ? qrRawProperty.GetString()
                : string.Empty;

            return new QrCodeData
            {
                RawData = qrRaw
            };
        }

        public FormattedReceipt FormatReceipt(JsonElement rawData)
        {
            if (!rawData.TryGetProperty("data", out var data) || !data.TryGetProperty("json", out var json))
            {
                return null;
            }

            var user = json.TryGetProperty("user", out var userProperty) ? userProperty.GetString() : null;
            var retailPlace = json.TryGetProperty("retailPlace", out var retailPlaceProperty) ? retailPlaceProperty.GetString() : null;

            var qrCodeData = ExtractQrCodeData(rawData);

            return new FormattedReceipt
            {
                User = user ?? "Неизвестный продавец",
                TotalSum = json.TryGetProperty("totalSum", out var totalSum) ? totalSum.GetDecimal() : 0m,
                DateTime = json.TryGetProperty("dateTime", out var dateTime) ? dateTime.GetString() ?? "Неизвестное время" : "Неизвестное время",
                RetailPlace = !string.IsNullOrWhiteSpace(retailPlace) ? retailPlace
                             : !string.IsNullOrWhiteSpace(user) ? user
                             : "Неизвестное место",
                Items = json.TryGetProperty("items", out var items)
                    ? items.EnumerateArray()
                        .Select(item => new ReceiptItem
                        {
                            Name = item.TryGetProperty("name", out var name) ? name.GetString() ?? "Неизвестный товар" : "Неизвестный товар",
                            Price = item.TryGetProperty("price", out var price) ? price.GetDecimal() : 0m,
                            Quantity = item.TryGetProperty("quantity", out var quantity) ? quantity.GetDecimal() : 0m,
                            Sum = item.TryGetProperty("sum", out var sum) ? sum.GetDecimal() : 0m,
                            Category = "Различные товары"
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