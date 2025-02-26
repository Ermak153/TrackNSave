namespace TrackNSave.Server.Models.DTOs
{
    public record RegisterRequest(string Username, string Email, string Password);
    public record LoginRequest(string Username, string Password);
}