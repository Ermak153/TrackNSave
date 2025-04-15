using System.Text.Json;
using TrackNSave.Server.Data;
using TrackNSave.Server.Models;
using TrackNSave.Server.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using TrackNSave.Server.Utils;
using DinkToPdf.Contracts;

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

        public async Task<List<ProductPriceHistory>> GetProductPriceHistoryAsync(Guid userId, string productName)
        {
            var receipts = await GetUserReceiptsAsync(userId);
            var productGroups = new Dictionary<string, ProductPriceHistory>();
            const double similarityThreshold = 0.8;

            foreach (var receipt in receipts)
            {
                var formattedReceipt = JsonSerializer.Deserialize<FormattedReceipt>(receipt.ReceiptData);
                if (formattedReceipt == null) continue;

                var uniquePricesInReceipt = new HashSet<decimal>();

                var matchingItems = formattedReceipt.Items
                    .Where(item => StringSimilarity.CalculateSimilarity(productName, item.Name) >= similarityThreshold)
                    .OrderByDescending(item => StringSimilarity.CalculateSimilarity(productName, item.Name))
                    .ToList();

                foreach (var item in matchingItems)
                {
                    if (!uniquePricesInReceipt.Add(item.Price))
                        continue;

                    var bestMatch = productGroups.Keys
                        .Select(existingName => new { Name = existingName, Similarity = StringSimilarity.CalculateSimilarity(existingName, item.Name) })
                        .OrderByDescending(x => x.Similarity)
                        .FirstOrDefault();

                    string groupName;
                    if (bestMatch != null && bestMatch.Similarity >= similarityThreshold)
                    {
                        groupName = bestMatch.Name;
                    }
                    else
                    {
                        groupName = item.Name;
                        productGroups[groupName] = new ProductPriceHistory
                        {
                            ProductName = groupName,
                            PriceHistory = new List<ProductPriceRecord>()
                        };
                    }

                    productGroups[groupName].PriceHistory.Add(new ProductPriceRecord
                    {
                        Price = item.Price,
                        DateTime = formattedReceipt.DateTime,
                        RetailPlace = formattedReceipt.RetailPlace
                    });
                }
            }

            return productGroups.Values.ToList();
        }
    }
}