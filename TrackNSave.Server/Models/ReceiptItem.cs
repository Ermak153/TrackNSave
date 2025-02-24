namespace TrackNSave.Server.Models
{
    public class ReceiptItem
    {
        public string Name { get; set; } = string.Empty;
        public int Price { get; set; }
        public int Quantity { get; set; }
        public int Sum { get; set; }
    }
}