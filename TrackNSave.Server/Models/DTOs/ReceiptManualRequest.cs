namespace TrackNSave.Server.Models.DTOs
{
    public class ReceiptManualRequest
    {
        public string RetailPlace { get; set; }
        public string DateTime { get; set; }
        public decimal TotalSum { get; set; }
        public List<ReceiptItem> Items { get; set; } = new List<ReceiptItem>();
    }
}
