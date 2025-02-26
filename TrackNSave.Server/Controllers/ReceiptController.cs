using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using TrackNSave.Server.Models;
using TrackNSave.Server.Services;
using TrackNSave.Server.Services.Implementations;
using TrackNSave.Server.Services.Interfaces;

namespace TrackNSave.Server.Controllers
{
    [ApiController]
    [Route("api/receipt/")]
    [Authorize]
    public class ReceiptsController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IReceiptService _receiptService;
        private readonly IReceiptApiService _receiptApiService;
        private readonly IReceiptParserService _receiptParserService;

        public ReceiptsController(IUserService userService, IReceiptService receiptService, IReceiptApiService receiptApiService, IReceiptParserService receiptParserService)
        {
            _userService = userService;
            _receiptService = receiptService;
            _receiptApiService = receiptApiService;
            _receiptParserService = receiptParserService;
        }

        [HttpGet("list")]
        public async Task<IActionResult> GetUserReceipts()
        {
            try
            {
                var token = Request.Cookies["auth_token"];
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
                    ReceiptData = JsonSerializer.Deserialize<FormattedReceipt>(r.ReceiptData)
                }).ToList();

                return StatusCode(200, new { formattedReceipts });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Error when receiving receipts" });
            }
        }

        [HttpPost("qrscan")]
        public async Task<IActionResult> ScanReceipt([FromBody] QrScanRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Qrraw))
            {
                return StatusCode(400, new { message = "Invalid QR code data" });
            }

            var token = Request.Cookies["auth_token"];
            var userId = await _userService.GetUserIdFromJwtAsync(token);
            if (string.IsNullOrEmpty(userId))
            {
                return StatusCode(401, new { message = "User is not authenticated" });
            }

            try
            {
                var rawData = await _receiptApiService.FetchReceiptDataAsync(request.Qrraw);
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

                var userGuid = Guid.Parse(userId);
                if (await _receiptService.ReceiptExistsAsync(userGuid, fiscalData))
                {
                    return StatusCode(409, new { message = "Receipt has already been added earlier" });
                }

                await _receiptService.SaveReceiptAsync(userGuid, formattedReceipt, fiscalData);
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
    }
}