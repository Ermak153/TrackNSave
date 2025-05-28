import { ref, computed } from 'vue';
import api from '@/api/axios';

interface User {
  id: string;
  username: string;
  email: string;
  createdAt: string;
  role?: string;
}

interface GetUsersResponse {
  users: User[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
}

interface AdminStats {
  totalUsers: number;
  totalReceipts: number;
  totalAmount: number;
}

interface UpdateUserRequest {
  userId: string;
  username: string;
  email: string;
}

interface GetUsersRequest {
  page: number;
  pageSize: number;
}

const users = ref<User[]>([]);
const totalUsersCount = ref(0);
const isLoadingUsers = ref(false);
const adminStats = ref<AdminStats>({
  totalUsers: 0,
  totalReceipts: 0,
  totalAmount: 0
});
const isLoadingStats = ref(false);

export function useAdmin() {
  const getAllUsers = async (page: number = 1, pageSize: number = 10) => {
    isLoadingUsers.value = true;
    try {
      const requestData: GetUsersRequest = {
        page,
        pageSize
      };

      const response = await api.post<GetUsersResponse>('/admin/users-list', requestData);

      users.value = response.data.users;
      totalUsersCount.value = response.data.totalCount;

      return {
        users: response.data.users,
        totalCount: response.data.totalCount,
        page: response.data.page,
        pageSize: response.data.pageSize,
        totalPages: response.data.totalPages
      };
    } catch (error) {
      console.error('Ошибка при получении пользователей:', error);
      users.value = [];
      totalUsersCount.value = 0;
      throw error;
    } finally {
      isLoadingUsers.value = false;
    }
  };

  const updateUser = async (userId: string, userData: { username: string; email: string }) => {
    try {
      const response = await api.put('/admin/user-edit', {
        userId: userId,
        username: userData.username,
        email: userData.email
      });
      return response.data;
    } catch (error) {
      console.error('Ошибка при обновлении пользователя:', error);
      throw error;
    }
  };

  const getAdminStats = async () => {
    isLoadingStats.value = true;
    try {
      const response = await api.get<AdminStats>('/admin/stats');

      const statsData = {
        ...response.data,
        totalAmount: response.data.totalAmount / 100
      };

      adminStats.value = statsData;
      return statsData;
    } catch (error) {
      console.error('Ошибка при получении статистики:', error);
      throw error;
    } finally {
      isLoadingStats.value = false;
    }
  };

  const totalUsers = computed(() => totalUsersCount.value);

  const isLoading = computed(() => isLoadingUsers.value || isLoadingStats.value);

  const formatDate = (dateString: string) => {
    const date = new Date(dateString);
    return date.toLocaleDateString('ru-RU', {
      year: 'numeric',
      month: '2-digit',
      day: '2-digit',
      hour: '2-digit',
      minute: '2-digit'
    });
  };

  const formatNumber = (num: number) => {
    return num.toLocaleString('ru-RU');
  };

  const formatCurrency = (amount: number) => {
    return amount.toLocaleString('ru-RU', {
      minimumFractionDigits: 2,
      maximumFractionDigits: 2
    });
  };

  const clearAdminData = () => {
    users.value = [];
    totalUsersCount.value = 0;
    adminStats.value = {
      totalUsers: 0,
      totalReceipts: 0,
      totalAmount: 0
    };
  };

  const updateLocalUser = (userId: string, updatedData: Partial<User>) => {
    const userIndex = users.value.findIndex(user => user.id === userId);
    if (userIndex !== -1) {
      users.value[userIndex] = { ...users.value[userIndex], ...updatedData };
    }
  };

  const removeLocalUser = (userId: string) => {
    users.value = users.value.filter(user => user.id !== userId);
    totalUsersCount.value = Math.max(0, totalUsersCount.value - 1);
  };

  return {
    users,
    totalUsers,
    totalUsersCount,
    adminStats,
    isLoadingUsers,
    isLoadingStats,
    isLoading,
    getAllUsers,
    updateUser,
    getAdminStats,
    formatDate,
    formatNumber,
    formatCurrency,
    clearAdminData,
    updateLocalUser,
    removeLocalUser,
  };
}

export type { User, AdminStats, UpdateUserRequest, GetUsersRequest };
