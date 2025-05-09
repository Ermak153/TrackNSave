import { ref } from 'vue';
import api from '@/api/axios';

const username = ref<string | null>(null);
const role = ref<string | null>(null);
const avatarUrl = ref<string | null>(null);

export function useUsers() {
  const getUserInfo = async () => {
    try {
      const response = await api.get('/user/me');
      username.value = response.data.username;
      role.value = response.data.role;
    } catch {
      username.value = null;
      role.value = null;
      avatarUrl.value = null;
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
      avatarUrl.value = URL.createObjectURL(response.data);
      console.log("Получен аватар")
    } catch (error) {
      console.error('Ошибка при получении аватара:', error);
      avatarUrl.value = null;
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

  return {
    getUserInfo,
    uploadAvatar,
    getAvatar,
    deleteAvatar,
    username,
    role,
    avatarUrl
  };
}
