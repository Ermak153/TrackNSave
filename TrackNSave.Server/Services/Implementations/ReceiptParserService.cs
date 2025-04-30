using System.Text.Json;
using TrackNSave.Server.Models;
using TrackNSave.Server.Services.Interfaces;

namespace TrackNSave.Server.Services.Implementations
{
    public class ReceiptParserException : Exception
    {
        public int StatusCode { get; }
        public ReceiptParserException(int statusCode, string message) : base(message) { StatusCode = statusCode; }
    }

    public class ReceiptParserService : IReceiptParserService
    {
        private readonly IReceiptApiService _receiptApiService;

        public ReceiptParserService(IReceiptApiService receiptApiService) 
        {
            _receiptApiService = receiptApiService;
        }

        public FiscalData ExtractFiscalData(JsonElement rawData)
        {
            if (rawData.ValueKind == JsonValueKind.Null || rawData.ValueKind == JsonValueKind.Undefined)
            {
                throw new ArgumentNullException(nameof(rawData), "Input JSON data cannot be null");
            }

            try
            {
                if (!rawData.TryGetProperty("data", out var data) ||
                    !data.TryGetProperty("json", out var json))
                {
                    return new FiscalData();
                }

                var fiscalData = new FiscalData
                {
                    FiscalSign = ExtractString(json, "fiscalSign") ?? string.Empty,
                    FiscalDriveNumber = ExtractString(json, "fiscalDriveNumber") ?? string.Empty,
                    FiscalDocumentNumber = ExtractString(json, "fiscalDocumentNumber") ?? string.Empty
                };

                return fiscalData;
            }
            catch (Exception)
            {
                throw new ReceiptParserException(500, "Failed to extract fiscal data");
            }
        }

        public QrCodeData ExtractQrCodeData(JsonElement rawData)
        {
            if (rawData.ValueKind == JsonValueKind.Null || rawData.ValueKind == JsonValueKind.Undefined)
            {
                throw new ArgumentNullException(nameof(rawData), "Input JSON data cannot be null");
            }

            var result = new QrCodeData();

            try
            {
                if (!rawData.TryGetProperty("request", out var request) ||
                    request.ValueKind == JsonValueKind.Null ||
                    request.ValueKind == JsonValueKind.Undefined)
                {
                    return result;
                }

                if (request.TryGetProperty("qrraw", out var qrRawProperty) &&
                    qrRawProperty.ValueKind != JsonValueKind.Null &&
                    qrRawProperty.ValueKind != JsonValueKind.Undefined)
                {
                    result.RawData = qrRawProperty.GetString() ?? string.Empty;
                }

                return result;
            }
            catch (Exception)
            {
                throw new ReceiptParserException(500, "Failed to extract QR code data");
            }
        }

        public async Task<FormattedReceipt> FormatReceiptAsync(JsonElement rawData)
        {
            if (rawData.ValueKind == JsonValueKind.Null || rawData.ValueKind == JsonValueKind.Undefined)
            {
                throw new ArgumentNullException(nameof(rawData), "Input JSON data cannot be null");
            }

            try
            {
                if (!rawData.TryGetProperty("data", out var data) ||
                    !data.TryGetProperty("json", out var json) ||
                    json.ValueKind == JsonValueKind.Null)
                {
                    throw new ReceiptParserException(500, "Required JSON fields 'data' or 'json' are missing");
                }

                var user = json.TryGetProperty("user", out var userProperty) ? userProperty.GetString() : null;
                var retailPlace = json.TryGetProperty("retailPlace", out var retailPlaceProperty) ? retailPlaceProperty.GetString() : null;

                var qrCodeData = ExtractQrCodeData(rawData);

                var items = json.TryGetProperty("items", out var itemsJson)
                    ? itemsJson.EnumerateArray()
                        .Select(item => new ReceiptItem
                        {
                            Name = item.TryGetProperty("name", out var name) ? name.GetString() ?? "Неизвестный товар" : "Неизвестный товар",
                            Price = item.TryGetProperty("price", out var price) ? price.GetDecimal() : 0m,
                            Quantity = item.TryGetProperty("quantity", out var quantity) ? quantity.GetDecimal() : 0m,
                            Sum = item.TryGetProperty("sum", out var sum) ? sum.GetDecimal() : 0m,
                            Category = "Различные товары"
                        })
                        .ToList()
                    : new List<ReceiptItem>();

                var categories = await _receiptApiService.FetchProductCategoryAsync(items.Select(i => i.Name).ToList());
                for (int i = 0; i < items.Count; i++)
                {
                    items[i].Category = categories.ElementAtOrDefault(i) ?? "Различные товары";
                }

                return new FormattedReceipt
                {
                    User = user ?? "Неизвестный продавец",
                    TotalSum = json.TryGetProperty("totalSum", out var totalSum) ? totalSum.GetDecimal() : 0m,
                    DateTime = json.TryGetProperty("dateTime", out var dateTime) ? dateTime.GetString() ?? "Неизвестное время" : "Неизвестное время",
                    RetailPlace = !string.IsNullOrWhiteSpace(retailPlace) ? retailPlace
                                 : !string.IsNullOrWhiteSpace(user) ? user
                                 : "Неизвестное место",
                    Items = items
                };
            }
            catch (Exception)
            {
                throw new ReceiptParserException(500, "Failed to format receipt data");
            }
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