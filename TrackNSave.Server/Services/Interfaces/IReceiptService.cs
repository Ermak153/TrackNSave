using TrackNSave.Server.Models;

namespace TrackNSave.Server.Services.Interfaces
{
    public interface IReceiptService
    {
        Task<List<Receipt>> GetUserReceiptsAsync(Guid userId);
        Task<bool> ReceiptExistsAsync(Guid userId, FiscalData fiscalData);
        Task SaveReceiptAsync(Guid userId, FormattedReceipt formattedReceipt, FiscalData fiscalData);
    }
}