namespace TrackNSave.Server.Services.Interfaces
{
    public interface IPasswordService
    {
        void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt);
        bool VerifyPassword(string password, byte[] storedHash, byte[] storedSalt);
    }
}