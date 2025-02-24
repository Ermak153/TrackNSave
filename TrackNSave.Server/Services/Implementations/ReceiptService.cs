using System.Text.Json;
using TrackNSave.Server.Data;
using TrackNSave.Server.Models;
using TrackNSave.Server.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace TrackNSave.Server.Services.Implementations
{
    public class ReceiptService : IReceiptService
    {
        private readonly ApplicationDbContext _context;

        public ReceiptService(ApplicationDbContext context)
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

        public async Task SaveReceiptAsync(Guid userId, FormattedReceipt formattedReceipt, FiscalData fiscalData)
        {
            var receipt = new Receipt
            {
                UserId = userId,
                ReceiptData = JsonSerializer.Serialize(formattedReceipt),
                FiscalSign = fiscalData.FiscalSign,
                FiscalDriveNumber = fiscalData.FiscalDriveNumber,
                FiscalDocumentNumber = fiscalData.FiscalDocumentNumber,
                CreatedAt = DateTime.UtcNow
            };

            _context.Receipts.Add(receipt);
            await _context.SaveChangesAsync();
        }
    }
}