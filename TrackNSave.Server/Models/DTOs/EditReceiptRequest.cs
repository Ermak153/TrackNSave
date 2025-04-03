using TrackNSave.Server.Models;

namespace TrackNSave.Server.Models.DTOs
{
    public class EditReceiptRequest
    {
        public int ReceiptId { get; set; }
        public FormattedReceipt Receipt { get; set; }
    }
} 