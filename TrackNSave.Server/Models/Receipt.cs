using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrackNSave.Server.Models
{
    public class Receipt
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public Guid UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public User User { get; set; } = null!;

        [Column(TypeName = "jsonb")]
        public string ReceiptData { get; set; } = null!;

        [Required]
        public string FiscalSign { get; set; } = null!;

        [Required]
        public string FiscalDriveNumber { get; set; } = null!;

        [Required]
        public string FiscalDocumentNumber { get; set; } = null!;
        [Required]
        public string QrCodeData {  get; set; } = null!;
        [Required]
        public bool IsVerified { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}