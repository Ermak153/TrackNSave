namespace TrackNSave.Server.Models.DTOs
{
    public class UpdateEmailRequest
    {
        public Guid UserId { get; set; }
        public string NewEmail { get; set; } = string.Empty;
    }
}
