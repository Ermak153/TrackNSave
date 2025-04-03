using System.Text.Json;
using TrackNSave.Server.Data;
using TrackNSave.Server.Models;
using TrackNSave.Server.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Npgsql.Internal;
using System.Reflection.Metadata;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.IO.Font.Constants;
using iText.Kernel.Font;
using iText.Kernel.Colors;
using iText.Barcodes;
using iText.IO.Font;
using DinkToPdf;
using DinkToPdf.Contracts;
using System;
using QRCoder;
using System.Globalization;

namespace TrackNSave.Server.Services.Implementations
{
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

        public async Task<Receipt?> GetReceiptByIdAsync(int receiptId)
        {
            return await _context.Receipts.FirstOrDefaultAsync(r => r.Id == receiptId);
        }

        public async Task<bool> DeleteReceiptAsync(int receiptId)
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

        public async Task<bool> UpdateReceiptAsync(int receiptId, FormattedReceipt formattedReceipt)
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
    }
}