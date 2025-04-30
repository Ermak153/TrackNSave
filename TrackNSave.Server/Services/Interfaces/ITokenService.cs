using System.Security.Claims;
using TrackNSave.Server.Models;

namespace TrackNSave.Server.Services.Interfaces
{
    public interface ITokenService
    {
        string GenerateAccessToken(IEnumerable<Claim> claims);
        string GenerateRefreshToken();
        bool ValidateToken(string token);
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
        Task<RefreshToken> SaveRefreshTokenAsync(Guid userId, string token, TimeSpan lifetime);
        Task RevokeRefreshTokenAsync(Guid userId, string token);
        Task<(RefreshToken Token, Guid UserId)> FindRefreshTokenAsync(string tokenValue);
    }
}
