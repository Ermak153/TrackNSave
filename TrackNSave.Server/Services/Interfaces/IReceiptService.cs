using System.Text.Json;
using TrackNSave.Server.Models;

namespace TrackNSave.Server.Services.Interfaces
{
    public interface IReceiptService
    {
        Task<List<Receipt>> GetUserReceiptsAsync(Guid userId);
        Task<bool> ReceiptExistsAsync(Guid userId, FiscalData fiscalData);
        Task SaveReceiptAsync(Guid userId, FormattedReceipt formattedReceipt, FiscalData fiscalData, QrCodeData qrCodeData, bool isVerified);
        Task<Receipt?> GetReceiptByIdAsync(int receiptId);
        Task<bool> DeleteReceiptAsync(int receiptId);
        Task<bool> UpdateReceiptAsync(int receiptId, FormattedReceipt formattedReceipt);
        Task<List<ProductPriceHistory>> GetProductPriceHistoryAsync(Guid userId, string productName);
    }
}