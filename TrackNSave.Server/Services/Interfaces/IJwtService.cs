using TrackNSave.Server.Models;

namespace TrackNSave.Server.Services.Interfaces
{
    public interface IJwtService
    {
        string GenerateToken(User user);
        string? GetUsernameFromToken(string token);
        bool ValidateToken(string token);
    }
}