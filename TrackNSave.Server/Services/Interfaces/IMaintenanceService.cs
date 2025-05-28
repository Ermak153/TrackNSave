namespace TrackNSave.Server.Services.Interfaces
{
    public interface IMaintenanceService
    {
        Task<bool> IsMaintenanceModeAsync();
        Task SetMaintenanceModeAsync(bool isEnabled);
        Task ToggleMaintenanceModeAsync();
        Task<bool> EmergencyToggleMaintenanceModeAsync(string token);
    }
}
