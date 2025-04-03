using System.Text.Json;
using TrackNSave.Server.Services.Interfaces;
using DinkToPdf;
using DinkToPdf.Contracts;
using QRCoder;
using System.Globalization;

namespace TrackNSave.Server.Services.Implementations
{
    public class ReceiptPdfException : Exception
    {
        public int StatusCode { get; }
        public ReceiptPdfException(int statusCode, string message) : base(message) { StatusCode = statusCode; }
    }
    public class ReceiptPdfService : IReceiptPdfService
    {
        private readonly IConverter _converter;
        private readonly string _templatePath;

        public ReceiptPdfService(IConverter converter)
        {
            _converter = converter;
            _templatePath = Path.Combine(AppContext.BaseDirectory, "Resources", "Templates", "ReceiptTemplate.html");
        }

        public async Task<byte[]> GenerateReceiptPdfAsync(JsonDocument rawData)
        {
            try
            {
                var jsonElement = rawData.RootElement.GetProperty("data").GetProperty("json");

                var templateData = PreprocessReceiptData(jsonElement);

                if (rawData.RootElement.TryGetProperty("request", out var qrRequestElement) &&
                    qrRequestElement.TryGetProperty("qrraw", out var qrrawElement))
                {
                    string qrData = qrrawElement.GetString();
                    if (!string.IsNullOrWhiteSpace(qrData))
                    {
                        templateData["qrData"] = qrData;
                        templateData["qrImageBase64"] = GenerateQrCodeBase64(qrData);
                    }
                }

                string htmlTemplate = await File.ReadAllTextAsync(_templatePath);
                string filledTemplate = RenderTemplate(htmlTemplate, templateData);

                var doc = new HtmlToPdfDocument()
                {
                    GlobalSettings = {
                    PaperSize = PaperKind.A4,
                    Orientation = Orientation.Portrait,
                    Margins = new MarginSettings { Top = 10, Bottom = 10, Left = 10, Right = 10 },
                },
                    Objects = {
                    new ObjectSettings {
                        HtmlContent = filledTemplate,
                        WebSettings = { DefaultEncoding = "utf-8" },
                    }
                }
                };

                return _converter.Convert(doc);
            }
            catch (Exception ex)
            {
                throw new ReceiptApiException(500, "Error generating receipt PDF");
            }
        }

        private Dictionary<string, object> PreprocessReceiptData(JsonElement jsonElement)
        {
            try
            {
                var data = new Dictionary<string, object>();

                data["user"] = jsonElement.GetProperty("user").GetString();
                data["retailPlaceAddress"] = jsonElement.GetProperty("retailPlaceAddress").GetString();
                data["requestNumber"] = jsonElement.GetProperty("requestNumber").GetInt32();
                if (jsonElement.TryGetProperty("operationType", out var operationTypeElement))
                {
                    int operationType = operationTypeElement.GetInt32();
                    string operationTypeName = operationType switch
                    {
                        1 => "Приход",
                        2 => "Возврат прихода",
                        3 => "Расход",
                        4 => "Возврат расхода",
                        _ => "Неизвестно"
                    };
                    data["operationTypeName"] = operationTypeName;
                }

                var items = new List<Dictionary<string, object>>();
                if (jsonElement.TryGetProperty("items", out var itemsElement))
                {
                    foreach (var item in itemsElement.EnumerateArray())
                    {
                        string name = item.GetProperty("name").GetString();
                        decimal price = item.GetProperty("price").GetInt32() / 100m;
                        decimal quantity = item.GetProperty("quantity").GetDecimal();
                        decimal sum = item.GetProperty("sum").GetInt32() / 100m;

                        items.Add(new Dictionary<string, object>
                        {
                            { "name", name },
                            { "price", price },
                            { "formattedPrice", price.ToString("F2", CultureInfo.InvariantCulture) },
                            { "quantity", quantity },
                            { "sum", sum },
                            { "formattedSum", sum.ToString("F2", CultureInfo.InvariantCulture) }
                        });
                    }
                }
                data["items"] = items;

                decimal totalSum = jsonElement.GetProperty("totalSum").GetInt32() / 100m;
                data["totalSum"] = totalSum.ToString("F2", CultureInfo.InvariantCulture);

                decimal cashSum = jsonElement.GetProperty("cashTotalSum").GetInt32() / 100m;
                data["cashTotalSum"] = cashSum.ToString("F2", CultureInfo.InvariantCulture);

                decimal ecashSum = jsonElement.GetProperty("ecashTotalSum").GetInt32() / 100m;
                data["ecashTotalSum"] = ecashSum.ToString("F2", CultureInfo.InvariantCulture);

                decimal nds18 = jsonElement.GetProperty("nds18").GetInt32() / 100m;
                data["nds18"] = nds18.ToString("F2", CultureInfo.InvariantCulture);

                decimal nds10 = jsonElement.GetProperty("nds10").GetInt32() / 100m;
                data["nds10"] = nds10.ToString("F2", CultureInfo.InvariantCulture);

                data["operator"] = jsonElement.GetProperty("operator").GetString();
                data["retailPlace"] = jsonElement.GetProperty("retailPlace").GetString();
                data["shiftNumber"] = jsonElement.GetProperty("shiftNumber").GetInt32();

                if (DateTime.TryParse(jsonElement.GetProperty("dateTime").GetString(), out DateTime dateTime))
                {
                    data["dateTime"] = dateTime.ToString("dd.MM.yyyy HH:mm");
                }
                else
                {
                    data["dateTime"] = jsonElement.GetProperty("dateTime").GetString();
                }

                if (jsonElement.TryGetProperty("appliedTaxationType", out var taxTypeElement))
                {
                    int taxType = taxTypeElement.GetInt32();
                    string taxTypeName = taxType switch
                    {
                        1 => "ОСН",
                        2 => "УСН доход",
                        4 => "УСН доход-расход",
                        8 => "ЕНВД",
                        16 => "ЕСХН",
                        32 => "Патент",
                        _ => "Неизвестно"
                    };
                    data["taxTypeName"] = taxTypeName;
                }

                data["kktRegId"] = jsonElement.GetProperty("kktRegId").GetString().Trim();
                data["numberKkt"] = jsonElement.GetProperty("numberKkt").GetString().Trim();
                data["fiscalDriveNumber"] = jsonElement.GetProperty("fiscalDriveNumber").GetString();
                data["fiscalDocumentNumber"] = jsonElement.GetProperty("fiscalDocumentNumber").GetInt32();
                data["fiscalSign"] = jsonElement.GetProperty("fiscalSign").GetInt64();
                data["userInn"] = jsonElement.GetProperty("userInn").GetString().Trim();

                return data;
            }
            catch (Exception)
            {
                throw new ReceiptPdfException(500, "Error parsing JSON");
            }
        }

        private string GenerateQrCodeBase64(string qrData)
        {
            try
            {
                var generator = new QRCoder.PayloadGenerator.Url(qrData);
                string payload = generator.ToString();

                var qrGenerator = new QRCodeGenerator();
                var qrCodeData = qrGenerator.CreateQrCode(payload, QRCodeGenerator.ECCLevel.Q);

                var pngByteQrCode = new PngByteQRCode(qrCodeData);
                byte[] qrCodeBytes = pngByteQrCode.GetGraphic(5);

                return Convert.ToBase64String(qrCodeBytes);
            }
            catch (Exception)
            {
                throw new ReceiptPdfException(500, "Error generating QRCode");
            }
        }

        private string RenderTemplate(string template, Dictionary<string, object> data)
        {
            try
            {
                string result = template;

                if (data.ContainsKey("items") && data["items"] is ICollection<Dictionary<string, object>> items)
                {
                    string startTag = "{{#items}}";
                    string endTag = "{{/items}}";
                    int startPos = result.IndexOf(startTag);
                    int endPos = result.IndexOf(endTag);

                    if (startPos >= 0 && endPos >= 0)
                    {
                        string itemTemplate = result.Substring(
                            startPos + startTag.Length,
                            endPos - startPos - startTag.Length);

                        string itemsContent = "";
                        foreach (var item in items)
                        {
                            string itemContent = itemTemplate;
                            foreach (var itemKey in item.Keys)
                            {
                                itemContent = itemContent.Replace($"{{{{{itemKey}}}}}", item[itemKey]?.ToString() ?? "");
                            }
                            itemsContent += itemContent;
                        }

                        result = result.Replace(result.Substring(startPos, endPos + endTag.Length - startPos), itemsContent);
                    }
                }

                foreach (var key in data.Keys)
                {
                    if (!(data[key] is ICollection<object>))
                    {
                        result = result.Replace($"{{{{{key}}}}}", data[key]?.ToString() ?? "");
                    }
                }

                return result;
            }
            catch (Exception)
            {
                throw new ReceiptPdfException(500, "Error rendering receipt template");
            }
        }
    }
}
