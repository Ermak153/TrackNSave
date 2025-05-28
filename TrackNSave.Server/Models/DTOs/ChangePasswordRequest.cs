using System.ComponentModel.DataAnnotations;

namespace TrackNSave.Server.Models.DTOs
{
    public class ChangePasswordRequest
    {
        [Required]
        public string CurrentPassword { get; set; }
        [Required]
        public string NewPassword { get; set; }
    }
}
