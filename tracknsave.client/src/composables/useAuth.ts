import { ref } from 'vue';
import api from '@/api/axios';

const isAuthenticated = ref(false);
const username = ref<string | null>(null);

export function useAuth() {
  const checkAuthStatus = async () => {
    try {
      const response = await api.get('/user/status');
      isAuthenticated.value = response.data.isAuthenticated;
    } catch (err) {
      console.error('Ошибка при проверке авторизации:', err);
      isAuthenticated.value = false;
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

  const getUserInfo = async () => {
    try {
      const response = await api.get('/user/me');
      username.value = response.data.username;
    } catch (err) {
      console.error('Ошибка при получении данных пользователя:', err);
      username.value = null;
    }
  };

  return {
    checkAuthStatus,
    getUserInfo,
    logout,
    isAuthenticated,
    username
  };
}
