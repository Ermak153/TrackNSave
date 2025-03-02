namespace TrackNSave.Server.Models
{
    public class ReceiptItem
    {
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public decimal Quantity { get; set; }
        public decimal Sum { get; set; }
    }
}