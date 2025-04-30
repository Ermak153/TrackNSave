using System.Text.Json;
using TrackNSave.Server.Data;
using TrackNSave.Server.Models;
using TrackNSave.Server.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using TrackNSave.Server.Utils;
using DinkToPdf.Contracts;

namespace TrackNSave.Server.Services.Implementations
{
    public class ReceiptServiceException : Exception
    {
        public int StatusCode { get; }
        public ReceiptServiceException(int statusCode, string message) : base(message) { StatusCode = statusCode; }
    }

    public class ReceiptService : IReceiptService
    {
        private readonly ApplicationDbContext _context;

        public ReceiptService(ApplicationDbContext context, IConverter converter)
        {
            _context = context;
        }

        public async Task<List<Receipt>> GetUserReceiptsAsync(Guid userId)
        {
            return await _context.Receipts
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<bool> ReceiptExistsAsync(Guid userId, FiscalData fiscalData)
        {
            return await _context.Receipts.AnyAsync(r =>
                r.UserId == userId &&
                r.FiscalSign == fiscalData.FiscalSign &&
                r.FiscalDriveNumber == fiscalData.FiscalDriveNumber &&
                r.FiscalDocumentNumber == fiscalData.FiscalDocumentNumber);
        }

        public async Task SaveReceiptAsync(Guid userId, FormattedReceipt formattedReceipt, FiscalData fiscalData, QrCodeData qrCodeData, bool isVerified)
        {
            try
            {
                if (formattedReceipt == null) throw new ReceiptServiceException(400, "Formatted receipt data cannot be null");
                if (fiscalData == null) throw new ReceiptServiceException(400, "Fiscal data cannot be null");
                if (qrCodeData == null) throw new ReceiptServiceException(400, "QR code data cannot be null");

                var receipt = new Receipt
                {
                    UserId = userId,
                    ReceiptData = JsonSerializer.Serialize(formattedReceipt),
                    FiscalSign = fiscalData.FiscalSign,
                    FiscalDriveNumber = fiscalData.FiscalDriveNumber,
                    FiscalDocumentNumber = fiscalData.FiscalDocumentNumber,
                    QrCodeData = qrCodeData.RawData,
                    CreatedAt = DateTime.UtcNow,
                    IsVerified = isVerified
                };

                _context.Receipts.Add(receipt);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw new ReceiptServiceException(500, "An unexpected error occurred while saving the receipt");
            }
        }

        public async Task<Receipt?> GetReceiptByIdAsync(int receiptId)
        {
            return await _context.Receipts.FirstOrDefaultAsync(r => r.Id == receiptId);
        }

        public async Task<bool> DeleteReceiptAsync(int receiptId)
        {
            try
            {
                var receipt = await _context.Receipts.FirstOrDefaultAsync(r => r.Id == receiptId);
                if (receipt == null)
                {
                    return false;
                }

                _context.Receipts.Remove(receipt);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                throw new ReceiptServiceException(500, "An unexpected error occurred while deleting the receipt");
            }
        }

        public async Task<bool> UpdateReceiptAsync(int receiptId, FormattedReceipt formattedReceipt)
        {
            try
            {
                var receipt = await _context.Receipts.FirstOrDefaultAsync(r => r.Id == receiptId);
                if (receipt == null)
                {
                    return false;
                }

                receipt.ReceiptData = JsonSerializer.Serialize(formattedReceipt);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                throw new ReceiptServiceException(500, "An unexpected error occurred while updating the receipt");
            }
        }
    }
}