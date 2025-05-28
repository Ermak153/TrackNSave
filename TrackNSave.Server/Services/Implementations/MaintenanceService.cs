using Microsoft.Extensions.Caching.Memory;
using TrackNSave.Server.Services.Interfaces;

namespace TrackNSave.Server.Services.Implementations
{
    public class MaintenanceService : IMaintenanceService
    {
        private readonly IMemoryCache _cache;
        private const string CACHE_KEY = "maintenance_mode";
        private const string FILE_PATH = "maintenance.txt";

        public MaintenanceService(IMemoryCache cache)
        {
            _cache = cache;
        }

        public async Task<bool> IsMaintenanceModeAsync()
        {
            if (_cache.TryGetValue(CACHE_KEY, out bool cachedValue))
            {
                return cachedValue;
            }

            bool isEnabled = false;
            if (File.Exists(FILE_PATH))
            {
                var content = await File.ReadAllTextAsync(FILE_PATH);
                bool.TryParse(content.Trim(), out isEnabled);
            }

            _cache.Set(CACHE_KEY, isEnabled, TimeSpan.FromMinutes(5));
            return isEnabled;
        }

        public async Task SetMaintenanceModeAsync(bool isEnabled)
        {
            await File.WriteAllTextAsync(FILE_PATH, isEnabled.ToString());
            _cache.Remove(CACHE_KEY);
        }

        public async Task ToggleMaintenanceModeAsync()
        {
            var currentStatus = await IsMaintenanceModeAsync();
            await SetMaintenanceModeAsync(!currentStatus);
        }

        public async Task<bool> EmergencyToggleMaintenanceModeAsync(string token)
        {
            var validToken = Environment.GetEnvironmentVariable("MAINTENANCE_TOKEN");

            if (string.IsNullOrEmpty(validToken) ||
                string.IsNullOrEmpty(token) ||
                validToken != token)
            {
                return false;
            }

            var currentStatus = await IsMaintenanceModeAsync();
            await SetMaintenanceModeAsync(!currentStatus);
            return true;
        }
    }
}
