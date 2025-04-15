using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using TrackNSave.Server.Models;
using TrackNSave.Server.Models.DTOs;
using TrackNSave.Server.Services.Implementations;
using TrackNSave.Server.Services.Interfaces;

namespace TrackNSave.Server.Controllers
{
    [ApiController]
    [Route("api/receipt/")]
    [Authorize]
    public class ReceiptsController(IUserService userService, IReceiptService receiptService, IReceiptApiService receiptApiService, IReceiptParserService receiptParserService, IReceiptPdfService receiptPdfService) : ControllerBase
    {
        private readonly IUserService _userService = userService;
        private readonly IReceiptService _receiptService = receiptService;
        private readonly IReceiptApiService _receiptApiService = receiptApiService;
        private readonly IReceiptParserService _receiptParserService = receiptParserService;
        private readonly IReceiptPdfService _receiptPdfService = receiptPdfService;

        [HttpGet("list")]
        public async Task<IActionResult> GetUserReceipts()
        {
            try
            {
                var token = Request.Cookies["auth_token"];

                if (token == null)
                {
                    return StatusCode(404, new { message = "Token not found" });
                }

                var userId = await _userService.GetUserIdFromJwtAsync(token);
                if (string.IsNullOrEmpty(userId))
                {
                    return StatusCode(401, new { message = "User is not authenticated" });
                }

                var receipts = await _receiptService.GetUserReceiptsAsync(Guid.Parse(userId));
                var formattedReceipts = receipts.Select(r => new
                {
                    r.Id,
                    r.CreatedAt,
                    r.IsVerified,
                    ReceiptData = JsonSerializer.Deserialize<FormattedReceipt>(r.ReceiptData)
                }).ToList();

                return StatusCode(200, new { formattedReceipts });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Error when receiving receipts" });
            }
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddReceipt([FromBody] ReceiptRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.ReceiptRaw))
            {
                return StatusCode(400, new { message = "Invalid receipt data" });
            }

            var token = Request.Cookies["auth_token"];

            if (token == null)
            {
                return StatusCode(404, new { message = "Token not found" });
            }

            var userId = await _userService.GetUserIdFromJwtAsync(token);
            if (string.IsNullOrEmpty(userId))
            {
                return StatusCode(401, new { message = "User is not authenticated" });
            }

            try
            {
                var rawData = await _receiptApiService.FetchReceiptDataAsync(request.ReceiptRaw);
                if (!rawData.HasValue)
                {
                    return StatusCode(400, new { message = "Couldn't get receipt details" });
                }

                var formattedReceipt = _receiptParserService.FormatReceipt(rawData.Value);

                if (formattedReceipt == null)
                {
                    return StatusCode(400, new { message = "Couldn't recognize the receipt" });
                }

                var fiscalData = _receiptParserService.ExtractFiscalData(rawData.Value);
                if (string.IsNullOrEmpty(fiscalData.FiscalSign) ||
                    string.IsNullOrEmpty(fiscalData.FiscalDriveNumber) ||
                    string.IsNullOrEmpty(fiscalData.FiscalDocumentNumber))
                {
                    return StatusCode(400, new { message = "Fiscal data not found" });
                }

                var qrCodeData = _receiptParserService.ExtractQrCodeData(rawData.Value);

                var userGuid = Guid.Parse(userId);
                if (await _receiptService.ReceiptExistsAsync(userGuid, fiscalData))
                {
                    return StatusCode(409, new { message = "Receipt has already been added earlier" });
                }

                await _receiptService.SaveReceiptAsync(userGuid, formattedReceipt, fiscalData, qrCodeData, true);
                return StatusCode(200, new { message = "Receipt was saved successfully" });
            }
            catch (HttpRequestException)
            {
                return StatusCode(500, new { message = "Error when receiving receipt data" });
            }
            catch (ReceiptApiException ex)
            {
                return StatusCode(ex.StatusCode, new { message = ex.Message });
            }
            catch (JsonException)
            {
                return StatusCode(500, new { message = "Error processing receipt data" });
            }
        }

        [HttpPost("manual-add")]
        public async Task<IActionResult> AddManualReceipt([FromBody] FormattedReceipt request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.RetailPlace) ||
                string.IsNullOrWhiteSpace(request.DateTime) || request.Items == null || request.Items.Count == 0)
            {
                return StatusCode(400, new { message = "Invalid receipt" });
            }

            var token = Request.Cookies["auth_token"];

            if (token == null)
            {
                return StatusCode(404, new { message = "Token not found" });
            }

            var userId = await _userService.GetUserIdFromJwtAsync(token);
            if (string.IsNullOrEmpty(userId))
            {
                return StatusCode(401, new { message = "User is not authenticated" });
            }

            try
            {
                var userGuid = Guid.Parse(userId);

                var formattedReceipt = new FormattedReceipt
                {
                    User = userId,
                    RetailPlace = request.RetailPlace,
                    DateTime = request.DateTime,
                    TotalSum = request.TotalSum,
                    Items = request.Items.Where(i => !string.IsNullOrWhiteSpace(i.Name) && i.Price > 0).ToList()
                };

                var fiscalData = new FiscalData
                {
                    FiscalSign = "MANUAL",
                    FiscalDriveNumber = "MANUAL",
                    FiscalDocumentNumber = "MANUAL"
                };

                var qrCodeData = new QrCodeData
                {
                    RawData = "MANUAL"
                };

                await _receiptService.SaveReceiptAsync(userGuid, formattedReceipt, fiscalData, qrCodeData, false);
                return StatusCode(200, new { message = "Receipt was saved successfully" });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Error saving receipt" });
            }
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteReceipt([FromBody] ReceiptIdRequest request)
        {
            if (request == null || request.ReceiptId <= 0)
            {
                return StatusCode(400, new { message = "Invalid receipt ID" });
            }

            var token = Request.Cookies["auth_token"];

            if (token == null)
            {
                return StatusCode(404, new { message = "Token not found" });
            }

            var userId = await _userService.GetUserIdFromJwtAsync(token);
            if (string.IsNullOrEmpty(userId))
            {
                return StatusCode(401, new { message = "User is not authenticated" });
            }

            var userGuid = Guid.Parse(userId);
            var receipt = await _receiptService.GetReceiptByIdAsync(request.ReceiptId);

            if (receipt == null)
            {
                return StatusCode(404, new { message = "Receipt not found" });
            }

            if (receipt.UserId != userGuid)
            {
                return StatusCode(403, new { message = "User access denied" });
            }

            await _receiptService.DeleteReceiptAsync(request.ReceiptId);
            return StatusCode(200, new { message = "Receipt deleted successfully" });
        }

        [HttpPut("edit")]
        public async Task<IActionResult> EditReceipt([FromBody] EditReceiptRequest request)
        {
            if (request == null || request.ReceiptId <= 0 || request.Receipt == null)
            {
                return StatusCode(400, new { message = "Invalid receipt" });
            }

            var token = Request.Cookies["auth_token"];

            if (token == null)
            {
                return StatusCode(404, new { message = "Token not found" });
            }

            var userId = await _userService.GetUserIdFromJwtAsync(token);
            if (string.IsNullOrEmpty(userId) || token == null)
            {
                return StatusCode(401, new { message = "User is not authenticated" });
            }

            try
            {
                var userGuid = Guid.Parse(userId);
                var existingReceipt = await _receiptService.GetReceiptByIdAsync(request.ReceiptId);

                if (existingReceipt == null)
                {
                    return StatusCode(404, new { message = "Receipt not found" });
                }

                if (existingReceipt.UserId != userGuid)
                {
                    return StatusCode(403, new { message = "User access denied" });
                }

                if (existingReceipt.FiscalSign != "MANUAL" || 
                    existingReceipt.FiscalDriveNumber != "MANUAL" || 
                    existingReceipt.FiscalDocumentNumber != "MANUAL")
                {
                    return StatusCode(403, new { message = "Only manual receipts can be edited" });
                }

                var formattedReceipt = new FormattedReceipt
                {
                    User = userId,
                    RetailPlace = request.Receipt.RetailPlace,
                    DateTime = request.Receipt.DateTime,
                    TotalSum = request.Receipt.TotalSum,
                    Items = request.Receipt.Items.Where(i => !string.IsNullOrWhiteSpace(i.Name) && i.Price > 0).ToList()
                };

                await _receiptService.UpdateReceiptAsync(request.ReceiptId, formattedReceipt);
                return StatusCode(200, new { message = "Receipt was updated successfully" });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Error updating receipt" });
            }
        }

        [HttpPost("get-pdf")]
        public async Task<IActionResult> GetReceiptPdf([FromBody] ReceiptIdRequest request)
        {
            if (request == null || request.ReceiptId <= 0)
            {
                return StatusCode(400, new { message = "Invalid receipt ID" });
            }

            var token = Request.Cookies["auth_token"];

            if (token == null)
            {
                return StatusCode(404, new { message = "Token not found" });
            }

            var userId = await _userService.GetUserIdFromJwtAsync(token);

            if (string.IsNullOrEmpty(userId))
            {
                return StatusCode(401, new { message = "User is not authenticated" });
            }

            try
            {
                var userGuid = Guid.Parse(userId);
                var receipt = await _receiptService.GetReceiptByIdAsync(request.ReceiptId);

                if (request == null || request.ReceiptId <= 0 || receipt == null)
                {
                    return StatusCode(404, new { message = "Receipt not found" });
                }

                if (receipt.UserId != userGuid)
                {
                    return StatusCode(403, new { message = "User access denied" });
                }

                if (string.IsNullOrEmpty(receipt.QrCodeData))
                {
                    return StatusCode(400, new { message = "QR code data not available for this receipt" });
                }

                var rawData = await _receiptApiService.FetchReceiptDataAsync(receipt.QrCodeData);

                if (!rawData.HasValue)
                {
                    return StatusCode(502, new { message = "Error when receiving receipt data from API" });
                }

                using var doc = JsonDocument.Parse(rawData.Value.GetRawText());
                var pdfBytes = await _receiptPdfService.GenerateReceiptPdfAsync(doc);

                return File(pdfBytes, "application/pdf", $"receipt_{request.ReceiptId}.pdf");
            }
            catch (HttpRequestException)
            {
                return StatusCode(502, new { message = "Error when receiving receipt data from API" });
            }
            catch (ReceiptApiException ex)
            {
                return StatusCode(ex.StatusCode, new { message = ex.Message });
            }
            catch (ReceiptPdfException ex)
            {
                return StatusCode(ex.StatusCode, new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Error generating receipt PDF" });
            }
        }
        
        [HttpPost("product-history")]
        public async Task<IActionResult> GetProductPriceHistory([FromBody] ReceiptHistoryRequest productName)
        {
            var token = Request.Cookies["auth_token"];

            if (token == null)
            {
                return StatusCode(404, new { message = "Token not found" });
            }

            var userId = await _userService.GetUserIdFromJwtAsync(token);
            if (string.IsNullOrEmpty(userId))
            {
                return StatusCode(401, new { message = "User is not authenticated" });
            }

            if (productName == null || productName.ProductName == null)
            {
                return StatusCode(400, new { message = "Product name is required" });
            }

            var productHistory = await _receiptService.GetProductPriceHistoryAsync(Guid.Parse(userId), productName.ProductName);
            return StatusCode(200, new { productHistory });
        }
    }
}