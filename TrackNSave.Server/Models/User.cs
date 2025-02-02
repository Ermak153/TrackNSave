using System.ComponentModel.DataAnnotations;

namespace TrackNSave.Server.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        public string Username { get; set; } = null!;

        [Required, EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        public byte[] PasswordHash { get; set; } = null!;

        [Required]
        public byte[] PasswordSalt { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
