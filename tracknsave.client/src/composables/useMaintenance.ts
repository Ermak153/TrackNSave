import { ref, readonly } from 'vue';
import api from '@/api/axios.ts';

const isMaintenanceMode = ref(false);
const isLoading = ref(false);
const error = ref<string | null>(null);

export function useMaintenance() {
  const checkMaintenanceStatus = async (): Promise<boolean> => {
    try {
      error.value = null;
      const response = await api.get('/maintenance/status');
      isMaintenanceMode.value = response.data.isEnabled;
      return response.data.isEnabled;
    } catch {
      error.value = 'Ошибка при проверке режима обслуживания';
      return false;
    }
  };

    const toggleMaintenanceMode = async (): Promise<boolean> => {
    try {
      isLoading.value = true;
      error.value = null;

      const response = await api.post('/maintenance/toggle');

      if (response.data.success) {
        isMaintenanceMode.value = response.data.isEnabled;
        return true;
      }

      error.value = 'Ошибка при переключении режима обслуживания';
      return false;
    } catch {
      error.value = 'Ошибка при переключении режима обслуживания';
      return false;
    } finally {
      isLoading.value = false;
    }
  };

  const emergencyToggleMaintenanceMode = async (token: string): Promise<boolean> => {
    try {
      isLoading.value = true;
      error.value = null;

      const response = await api.post('/maintenance/emergency-toggle', { token });

      if (response.data.success) {
        isMaintenanceMode.value = response.data.isEnabled;
        return true;
      }

      error.value = 'Ошибка при экстренном переключении режима';
      return false;
    } catch {
      error.value = 'Неверный токен или ошибка сервера';
      return false;
    } finally {
      isLoading.value = false;
    }
  };

  const clearError = () => {
    error.value = null;
  };

  return {
    isMaintenanceMode: readonly(isMaintenanceMode),
    isLoading: readonly(isLoading),
    error: readonly(error),
    checkMaintenanceStatus,
    toggleMaintenanceMode,
    emergencyToggleMaintenanceMode,
    clearError
  };
}
