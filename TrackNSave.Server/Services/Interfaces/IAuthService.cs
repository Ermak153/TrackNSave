namespace TrackNSave.Server.Services.Interfaces
{
    public interface IAuthService
    {
        Task<string?> AuthenticateAsync(string username, string password);
        Task<bool> RegisterAsync(string username, string email, string password);
    }
}