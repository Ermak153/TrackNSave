namespace TrackNSave.Server.Models
{
    public class FormattedReceipt
    {
        public string User { get; set; } = string.Empty;
        public decimal TotalSum { get; set; }
        public string DateTime { get; set; } = string.Empty;
        public string RetailPlace { get; set; } = string.Empty;
        public List<ReceiptItem> Items { get; set; } = new List<ReceiptItem>();
    }
}