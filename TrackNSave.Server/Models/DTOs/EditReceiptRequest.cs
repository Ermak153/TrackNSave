using System.ComponentModel.DataAnnotations;

namespace TrackNSave.Server.Models.DTOs
{
    public class EditReceiptRequest
    {
        public int ReceiptId { get; set; }
        public required FormattedReceipt Receipt { get; set; }
    }
} 