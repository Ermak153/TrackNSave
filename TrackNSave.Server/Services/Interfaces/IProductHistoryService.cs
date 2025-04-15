using TrackNSave.Server.Models;

namespace TrackNSave.Server.Services.Interfaces
{
    public interface IProductHistoryService
    {
        Task<List<ProductPriceHistory>> GetProductPriceHistoryAsync(Guid userId, string productName);
    }
}
