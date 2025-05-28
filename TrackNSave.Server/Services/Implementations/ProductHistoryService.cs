using System.Text.Json;
using TrackNSave.Server.Models;
using TrackNSave.Server.Models.DTOs;
using TrackNSave.Server.Services.Interfaces;
using TrackNSave.Server.Utils;

namespace TrackNSave.Server.Services.Implementations
{
    public class ProductHistoryException : Exception
    {
        public int StatusCode { get; }
        public ProductHistoryException(int statusCode, string message) : base(message) { StatusCode = statusCode; }
    }

    public class ProductHistoryService : IProductHistoryService
    {
        private readonly IReceiptService _receiptService;

        public ProductHistoryService(IReceiptService receiptService)
        {
            _receiptService = receiptService;
        }

        public async Task<List<ProductPriceHistory>> GetProductPriceHistoryAsync(Guid userId, string productName)
        {
            var receipts = await _receiptService.GetUserReceiptsAsync(userId);

            if (receipts == null || !receipts.Any())
            {
                throw new ProductHistoryException(404, "Receipts not found");
            }

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

