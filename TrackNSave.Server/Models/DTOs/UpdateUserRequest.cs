namespace TrackNSave.Server.Models.DTOs
{
    public class UpdateUserRequest
    {
        public Guid UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}
