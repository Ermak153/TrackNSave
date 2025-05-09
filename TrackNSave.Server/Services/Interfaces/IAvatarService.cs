using Microsoft.AspNetCore.Mvc;

namespace TrackNSave.Server.Services.Interfaces
{
    public interface IAvatarService
    {
        Task<string?> SaveAvatarAsync(IFormFile file, Guid userId);
        Task<bool> DeleteAvatarAsync(Guid userId, string? currentAvatarFileName);
        Task<FileStreamResult?> GetAvatarAsync(Guid userId, string? avatarFileName);
    }
}
