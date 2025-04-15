namespace TrackNSave.Server.Models
{
    public class ProductPriceHistory
    {
        public string ProductName { get; set; } = string.Empty;
        public List<ProductPriceRecord> PriceHistory { get; set; } = new List<ProductPriceRecord>();
    }

    public class ProductPriceRecord
    {
        public decimal Price { get; set; }
        public string DateTime { get; set; } = string.Empty;
        public string RetailPlace { get; set; } = string.Empty;
    }
} 