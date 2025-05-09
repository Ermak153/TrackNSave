using Microsoft.AspNetCore.Mvc;
using TrackNSave.Server.Services.Interfaces;

namespace TrackNSave.Server.Services.Implementations
{
    public class AvatarServiceException : Exception
    {
        public int StatusCode { get; }
        public AvatarServiceException(int statusCode, string message) : base(message) { StatusCode = statusCode; }
    }

    public class AvatarService : IAvatarService
    {
        private readonly IWebHostEnvironment _env;

        private const string AvatarDirectory = "Avatars";
        private const int MaxFileSizeMB = 5;
        private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".gif" };

        public AvatarService(IWebHostEnvironment env)
        {
            _env = env;
        }

        public async Task<string?> SaveAvatarAsync(IFormFile file, Guid userId)
        {
            try
            {
                if (file.Length > MaxFileSizeMB * 1024 * 1024)
                {
                    throw new AvatarServiceException(400, "File size exceeds 5MB limit");
                }

                var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (!AllowedExtensions.Contains(fileExtension))
                {
                    throw new AvatarServiceException(400, "Invalid file extension");
                }

                var avatarDir = Path.Combine(_env.WebRootPath, AvatarDirectory);
                await Task.Run(() => Directory.CreateDirectory(avatarDir));

                var newFileName = $"{Guid.NewGuid()}{fileExtension}";
                var filePath = Path.Combine(avatarDir, newFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                return newFileName;
            }
            catch (Exception)
            {
                throw new AvatarServiceException(500, "Error saving avatar");
            }
        }

        public async Task<bool> DeleteAvatarAsync(Guid userId, string? currentAvatarFileName)
        {
            if (string.IsNullOrEmpty(currentAvatarFileName))
                return true;

            try
            {
                var avatarDir = Path.Combine(_env.WebRootPath, AvatarDirectory);
                var filePath = Path.Combine(avatarDir, currentAvatarFileName);

                return await Task.Run(() =>
                {
                    if (File.Exists(filePath))
                    {
                        File.Delete(filePath);
                        return true;
                    }
                    return false;
                });
            }
            catch (Exception)
            {
                throw new AvatarServiceException(500, "Error deleting avatar");
            }
        }

        public async Task<FileStreamResult?> GetAvatarAsync(Guid userId, string? avatarFileName)
        {
            if (string.IsNullOrEmpty(avatarFileName))
                return null;

            var avatarDir = Path.Combine(_env.WebRootPath, AvatarDirectory);
            var filePath = Path.Combine(avatarDir, avatarFileName);

            var fileExists = await Task.Run(() => File.Exists(filePath));
            if (!fileExists)
                return null;

            var fileStream = await Task.Run(() => File.OpenRead(filePath));

            var fileExtension = Path.GetExtension(filePath).ToLowerInvariant();
            var contentType = fileExtension switch
            {
                ".jpg" => "image/jpeg",
                ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                _ => "application/octet-stream"
            };

            return new FileStreamResult(fileStream, contentType);
        }
    }
}
