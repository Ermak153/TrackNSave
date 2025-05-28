import { ref, computed } from 'vue';
import api from '@/api/axios';

const username = ref<string | null>(null);
const email = ref<string | null>(null);
const role = ref<string | null>(null);
const avatarUrl = ref<string | null>(null);
const registrationDate = ref<string | null>(null);

export function useUsers() {
  const getUserInfo = async () => {
    try {
      const response = await api.get('/user/me');
      username.value = response.data.username;
      email.value = response.data.email;
      role.value = response.data.role;
      registrationDate.value = response.data.registrationDate;
    } catch {
      username.value = null;
      email.value = null;
      role.value = null;
      avatarUrl.value = null;
      registrationDate.value = null;
    }
  };

  const uploadAvatar = async (file: File) => {
    try {
      const formData = new FormData();
      formData.append('file', file);

      const response = await api.post('/user/upload-avatar', formData, {
        headers: {
          'Content-Type': 'multipart/form-data'
        }
      });

      if (response.status === 200) {
        await getAvatar();
      } else {
        throw new Error('Ошибка при загрузке аватара');
      }
    } catch (error) {
      console.error('Ошибка при загрузке аватара:', error);
      throw error;
    }
  };

  const getAvatar = async () => {
    try {
      const response = await api.get('/user/get-avatar', {
        responseType: 'blob'
      });

      if (response.data.size === 0) {
        avatarUrl.value = null;
      } else {
        avatarUrl.value = URL.createObjectURL(response.data);
      }
    } catch (error) {
      avatarUrl.value = null;
      throw error;
    }
  };

  const deleteAvatar = async () => {
    try {
      await api.delete('/user/delete-avatar');
      avatarUrl.value = null;
    } catch (error) {
      console.error('Ошибка при удалении аватара:', error);
      throw error;
    }
  };

  const changePassword = async (currentPassword: string, newPassword: string) => {
    try {
      const response = await api.post('/user/change-password', {
        currentPassword,
        newPassword
      });
      return response.data;
    } catch (error) {
      console.error('Ошибка при смене пароля:', error);
      throw error;
    }
  };

  const formatRegistrationTime = computed(() => {
    if (!registrationDate.value) return "Неизвестно";

    const now = new Date();
    const regDate = new Date(registrationDate.value);
    const diffInMonths = (now.getFullYear() - regDate.getFullYear()) * 12
                      + (now.getMonth() - regDate.getMonth());

    const years = Math.floor(diffInMonths / 12);
    const months = diffInMonths % 12;

    const result = [];
    if (years > 0) result.push(`${years} ${getYearWord(years)}`);
    if (months > 0) result.push(`${months} ${getMonthWord(months)}`);

    return result.length > 0 ? result.join(' ') : "Менее месяца";
  });

  const getYearWord = (count: number) => {
    const lastDigit = count % 10;
    const lastTwoDigits = count % 100;

    if (lastTwoDigits >= 11 && lastTwoDigits <= 19) return 'лет';
    if (lastDigit === 1) return 'год';
    if (lastDigit >= 2 && lastDigit <= 4) return 'года';
    return 'лет';
  };

  const getMonthWord = (count: number) => {
    const lastDigit = count % 10;
    const lastTwoDigits = count % 100;

    if (lastTwoDigits >= 11 && lastTwoDigits <= 19) return 'месяцев';
    if (lastDigit === 1) return 'месяц';
    if (lastDigit >= 2 && lastDigit <= 4) return 'месяца';
    return 'месяцев';
  };

  return {
    getUserInfo,
    uploadAvatar,
    getAvatar,
    deleteAvatar,
    changePassword,
    username,
    email,
    role,
    avatarUrl,
    registrationTime: formatRegistrationTime
  };
}
