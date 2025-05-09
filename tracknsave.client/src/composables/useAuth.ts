import { ref } from 'vue';
import api from '@/api/axios';
import { useUsers } from "@/composables/useUsers.ts";
const { getUserInfo, username, role } = useUsers();

const isAuthenticated = ref(false);
const lastCheckTime = ref(0);
const isChecking = ref(false);

export function useAuth() {
  const checkAuthStatus = async (force = false): Promise<boolean> => {
    if (isChecking.value) {
      return isAuthenticated.value;
    }

    const now = Date.now();
    if (!force && (now - lastCheckTime.value < 30000)) {
      return isAuthenticated.value;
    }

    isChecking.value = true;

    try {
      const response = await api.get('/user/status');
      isAuthenticated.value = response.data.isAuthenticated;
      lastCheckTime.value = Date.now();

      if (isAuthenticated.value) {
        await getUserInfo();
        return true;
      } else {
        const refreshed = await refreshToken();

        if (refreshed) {
          const recheck = await api.get('/user/status');
          isAuthenticated.value = recheck.data.isAuthenticated;
          lastCheckTime.value = Date.now();

          if (isAuthenticated.value) {
            await getUserInfo();
            return true;
          }
        }

        clearAuthState();
        return false;
      }
    } catch (error) {
      console.error('Ошибка проверки аутентификации:', error);
      clearAuthState();
      return false;
    } finally {
      isChecking.value = false;
    }
  };

  const clearAuthState = () => {
    isAuthenticated.value = false;
    username.value = null;
    role.value = null;
  }

  const refreshToken = async (): Promise<boolean> => {
    try {
      await api.post('/auth/refresh');
      lastCheckTime.value = Date.now();
      return true;
    } catch (error) {
      console.error('Ошибка обновления токена:', error);
      clearAuthState();
      return false;
    }
  };

  const setupTokenRefreshTimer = () => {
    return setInterval(async () => {
      if (isAuthenticated.value) {
        await refreshToken();
      }
    }, 10 * 60 * 1000);
  };

  const login = async (credentials: { username: string; password: string }) => {
    try {
      const response = await api.post('/auth/login', credentials);
      isAuthenticated.value = true;
      lastCheckTime.value = Date.now();
      return response.data;
    } catch (error) {
      clearAuthState();
      throw error;
    }
  };

  const logout = async () => {
    try {
      await api.post('/auth/logout');
      isAuthenticated.value = false;
    } catch (err) {
      console.error('Ошибка при выходе из системы:', err);
    }
  };


  return {
    checkAuthStatus,
    logout,
    login,
    refreshToken,
    setupTokenRefreshTimer,
    isAuthenticated,
  };
}
