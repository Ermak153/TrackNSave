<template>
  <div class="admin">
    <div class="admin__container">
      <div class="admin__header">
        <h1 class="admin__title">Панель администратора</h1>
        <p class="admin__subtitle">Управление пользователями и системой</p>
      </div>

      <div v-if="isLoadingStats" class="admin__loading-container">
        <div class="loading-spinner">
          <div class="spinner"></div>
        </div>
        <p class="loading-text">Загрузка статистики...</p>
      </div>

      <div v-else class="admin__stats">
        <div class="info-card">
          <div class="info-card__icon-wrapper">
            <svg class="info-card__icon" xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
              <path d="M17 21v-2a4 4 0 0 0-4-4H5a4 4 0 0 0-4 4v2"></path>
              <circle cx="9" cy="7" r="4"></circle>
              <path d="M23 21v-2a4 4 0 0 0-3-3.87"></path>
              <path d="M16 3.13a4 4 0 0 1 0 7.75"></path>
            </svg>
          </div>
          <div class="info-card__details">
            <h3 class="info-card__value">{{ formatNumber(adminStats.totalUsers) }}</h3>
            <p class="info-card__label">Всего пользователей</p>
          </div>
        </div>

        <div class="info-card">
          <div class="info-card__icon-wrapper">
            <svg class="info-card__icon" xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
              <path d="M3 3v18h18"></path>
              <path d="M18 17v4"></path>
              <path d="M13 13v8"></path>
              <path d="M8 9v12"></path>
            </svg>
          </div>
          <div class="info-card__details">
            <h3 class="info-card__value">{{ formatNumber(adminStats.totalReceipts) }}</h3>
            <p class="info-card__label">Всего чеков</p>
          </div>
        </div>

        <div class="info-card">
          <div class="info-card__icon-wrapper">
            <svg class="info-card__icon" xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
              <circle cx="12" cy="12" r="10"></circle>
              <path d="M16 8h-6a2 2 0 1 0 0 4h4a2 2 0 1 1 0 4H8"></path>
              <path d="M12 18V6"></path>
            </svg>
          </div>
          <div class="info-card__details">
            <h3 class="info-card__value">{{ formatNumber(adminStats.totalAmount) }} ₽</h3>
            <p class="info-card__label">Общая сумма</p>
          </div>
        </div>
      </div>

      <div class="admin__sections">
        <div class="admin__section">
          <div class="admin__section-header">
            <h2 class="admin__section-title">Управление пользователями</h2>
            <div class="admin__users-info">
              Показано {{ startIndex + 1 }}-{{ Math.min(endIndex, totalUsers) }} из {{ totalUsers }}
            </div>
          </div>

          <div v-if="isLoadingUsers" class="admin__loading-container admin__loading-container--table">
            <div class="loading-spinner">
              <div class="spinner"></div>
            </div>
            <p class="loading-text">Загрузка пользователей...</p>
          </div>

          <div v-else class="admin__table-wrapper">
            <table class="admin__table">
              <thead>
                <tr>
                  <th>Имя пользователя</th>
                  <th>Email</th>
                  <th>Дата регистрации</th>
                  <th>Действия</th>
                </tr>
              </thead>
              <tbody>
                <tr v-for="user in paginatedUsers" :key="user.id">
                  <td>
                    <div v-if="editingUser?.id === user.id" class="admin__edit-field">
                      <input
                        v-model="editForm.username"
                        type="text"
                        class="admin__input"
                        @keyup.enter="saveUser"
                        @keyup.escape="cancelEdit"
                      />
                    </div>
                    <div v-else>{{ user.username }}</div>
                  </td>
                  <td>
                    <div v-if="editingUser?.id === user.id" class="admin__edit-field">
                      <input
                        v-model="editForm.email"
                        type="email"
                        class="admin__input"
                        @keyup.enter="saveUser"
                        @keyup.escape="cancelEdit"
                      />
                    </div>
                    <div v-else>{{ user.email }}</div>
                  </td>
                  <td>{{ formatDate(user.createdAt) }}</td>
                  <td>
                    <div v-if="editingUser?.id === user.id" class="admin__edit-actions">
                      <button
                        class="admin__action-btn admin__action-btn--success"
                        @click="saveUser"
                        :disabled="isSaving"
                      >
                        <span v-if="isSaving" class="button-spinner"></span>
                        {{ isSaving ? 'Сохранение...' : 'Сохранить' }}
                      </button>
                      <button
                        class="admin__action-btn"
                        @click="cancelEdit"
                        :disabled="isSaving"
                      >
                        Отмена
                      </button>
                    </div>
                    <div v-else>
                      <button
                        class="admin__action-btn"
                        @click="startEdit(user)"
                      >
                        Редактировать
                      </button>
                    </div>
                  </td>
                </tr>
              </tbody>
            </table>

            <div class="admin__mobile-cards">
              <div v-for="user in paginatedUsers" :key="user.id" class="admin__mobile-card">
                <div class="admin__mobile-row">
                  <span class="admin__mobile-label">Имя пользователя:</span>
                  <div v-if="editingUser?.id === user.id" class="admin__mobile-edit">
                    <input
                      v-model="editForm.username"
                      type="text"
                      class="admin__input admin__input--mobile"
                      @keyup.enter="saveUser"
                      @keyup.escape="cancelEdit"
                    />
                  </div>
                  <span v-else class="admin__mobile-value">{{ user.username }}</span>
                </div>
                <div class="admin__mobile-row">
                  <span class="admin__mobile-label">Email:</span>
                  <div v-if="editingUser?.id === user.id" class="admin__mobile-edit">
                    <input
                      v-model="editForm.email"
                      type="email"
                      class="admin__input admin__input--mobile"
                      @keyup.enter="saveUser"
                      @keyup.escape="cancelEdit"
                    />
                  </div>
                  <span v-else class="admin__mobile-value">{{ user.email }}</span>
                </div>
                <div class="admin__mobile-row">
                  <span class="admin__mobile-label">Дата регистрации:</span>
                  <span class="admin__mobile-value">{{ formatDate(user.createdAt) }}</span>
                </div>
                <div class="admin__mobile-actions">
                  <div v-if="editingUser?.id === user.id" class="admin__edit-actions">
                    <button
                      class="admin__action-btn admin__action-btn--success"
                      @click="saveUser"
                      :disabled="isSaving"
                    >
                      <span v-if="isSaving" class="button-spinner"></span>
                      {{ isSaving ? 'Сохранение...' : 'Сохранить' }}
                    </button>
                    <button
                      class="admin__action-btn"
                      @click="cancelEdit"
                      :disabled="isSaving"
                    >
                      Отмена
                    </button>
                  </div>
                  <div v-else>
                    <button
                      class="admin__action-btn"
                      @click="startEdit(user)"
                    >
                      Редактировать
                    </button>
                  </div>
                </div>
              </div>
            </div>
          </div>

          <div class="admin__pagination" v-if="!isLoadingUsers">
            <button
              class="admin__pagination-btn"
              :disabled="currentPage === 1"
              @click="goToPage(currentPage - 1)"
            >
              <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
                <path d="M15 18l-6-6 6-6"/>
              </svg>
              Предыдущая
            </button>

            <div class="admin__pagination-info">
              <span class="admin__pagination-pages">
                Страница {{ currentPage }} из {{ totalPages }}
              </span>
              <select
                v-model="itemsPerPage"
                class="admin__pagination-select"
                @change="changeItemsPerPage"
              >
                <option :value="10">10 на странице</option>
                <option :value="25">25 на странице</option>
                <option :value="50">50 на странице</option>
              </select>
            </div>

            <button
              class="admin__pagination-btn"
              :disabled="currentPage === totalPages"
              @click="goToPage(currentPage + 1)"
            >
              Следующая
              <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
                <path d="M9 18l6-6-6-6"/>
              </svg>
            </button>
          </div>
        </div>

        <div class="admin__section">
          <h2 class="admin__section-title">Системные настройки</h2>
          <div class="admin__settings">
            <div class="admin__setting-item">
              <div class="admin__setting-info">
                <svg class="admin__setting-icon" xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
                  <path d="M21 12.79A9 9 0 1 1 11.21 3 7 7 0 0 0 21 12.79z"></path>
                </svg>
                <span>Режим обслуживания</span>
              </div>
              <label class="switch">
                <input
                  type="checkbox"
                  :checked="maintenance.isMaintenanceMode.value"
                  @change="handleMaintenanceToggle"
                  :disabled="maintenance.isLoading.value"
                >
                <span class="slider round"></span>
              </label>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script lang="ts" setup>
import { ref, computed, onMounted, watch } from 'vue';
import { useMaintenance } from '@/composables/useMaintenance';
import { useAdmin, type User } from '@/composables/useAdmin';

const maintenance = useMaintenance();
const {
  users,
  totalUsers,
  adminStats,
  isLoadingUsers,
  isLoadingStats,
  getAllUsers,
  updateUser,
  getAdminStats,
  formatDate,
  updateLocalUser,
  formatNumber
} = useAdmin();

const currentPage = ref(1);
const itemsPerPage = ref(10);

const editingUser = ref<User | null>(null);
const isSaving = ref(false);
const editForm = ref({
  username: '',
  email: ''
});

const totalPages = computed(() => Math.ceil(totalUsers.value / itemsPerPage.value));
const startIndex = computed(() => (currentPage.value - 1) * itemsPerPage.value);
const endIndex = computed(() => currentPage.value * itemsPerPage.value);

const paginatedUsers = computed(() => {
  return users.value;
});

const fetchUsers = async () => {
  try {
    await getAllUsers(currentPage.value, itemsPerPage.value);
  } catch (error) {
    console.error('Ошибка при загрузке пользователей:', error);
  }
};

const fetchStats = async () => {
  try {
    await getAdminStats();
  } catch (error) {
    console.error('Ошибка при загрузке статистики:', error);
  }
};

const saveUser = async () => {
  if (!editingUser.value) return;

  if (!editForm.value.username.trim() || !editForm.value.email.trim()) {
    alert('Пожалуйста, заполните все поля');
    return;
  }

  isSaving.value = true;

  try {
    await updateUser(editingUser.value.id, {
      username: editForm.value.username.trim(),
      email: editForm.value.email.trim()
    });

    updateLocalUser(editingUser.value.id, {
      username: editForm.value.username.trim(),
      email: editForm.value.email.trim()
    });

    cancelEdit();
  } catch {
    console.error('Ошибка при сохранении пользователя');
  } finally {
    isSaving.value = false;
  }
};

const startEdit = (user: User) => {
  editingUser.value = user;
  editForm.value = {
    username: user.username,
    email: user.email
  };
};

const cancelEdit = () => {
  editingUser.value = null;
  editForm.value = {
    username: '',
    email: ''
  };
};

const goToPage = (page: number) => {
  if (page >= 1 && page <= totalPages.value) {
    currentPage.value = page;
  }
};

const changeItemsPerPage = () => {
  currentPage.value = 1;
};

onMounted(async () => {
  await Promise.all([
    maintenance.checkMaintenanceStatus(),
    fetchUsers(),
    fetchStats()
  ]);
});

watch([currentPage, itemsPerPage], () => {
  fetchUsers();
});

const handleMaintenanceToggle = async () => {
  await maintenance.toggleMaintenanceMode();
};
</script>


<style lang="scss" scoped>
.admin {
  padding: 0 20px;

  &__container {
    width: 100%;
    max-width: 1280px;
    margin: 0 auto;
    box-sizing: border-box;
  }

  &__loading-container {
    display: flex;
    flex-direction: column;
    align-items: center;
    justify-content: center;
    padding: 60px 20px;
    background-color: var(--vt-c-dark-blue-gray);
    border-radius: 16px;
    margin-bottom: 32px;
    box-shadow: 0 4px 6px -1px rgba(0, 0, 0, 0.1);

    &--table {
      margin-bottom: 20px;
      padding: 80px 20px;
      background-color: rgba(255, 255, 255, 0.02);
      border: 1px solid rgba(255, 255, 255, 0.05);
    }

    @media (max-width: 768px) {
      padding: 40px 20px;

      &--table {
        padding: 60px 20px;
      }
    }
  }

  &__header {
    margin-bottom: 32px;
    background-color: var(--vt-c-dark-blue-gray);
    border-radius: 16px;
    padding: 24px;
    box-shadow: 0 4px 6px -1px rgba(0, 0, 0, 0.1);
  }

  &__title {
    font-size: 24px;
    font-weight: 600;
    color: var(--vt-c-white);
    margin: 0 0 8px 0;
  }

  &__subtitle {
    font-size: 16px;
    color: var(--vt-c-light-gray);
    margin: 0;
  }

  &__stats {
    display: grid;
    grid-template-columns: repeat(3, 1fr);
    gap: 20px;
    margin-bottom: 32px;

    @media (max-width: 768px) {
      grid-template-columns: 1fr;
    }
  }

  &__sections {
    display: grid;
    gap: 32px;
  }

  &__section {
    background-color: var(--vt-c-dark-blue-gray);
    border-radius: 16px;
    padding: 24px;
    box-shadow: 0 4px 6px -1px rgba(0, 0, 0, 0.1);

    @media (max-width: 768px) {
      padding: 16px;
    }
  }

  &__section-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
    margin-bottom: 20px;

    @media (max-width: 768px) {
      flex-direction: column;
      align-items: flex-start;
      gap: 8px;
      margin-bottom: 16px;
    }
  }

  &__section-title {
    font-size: 18px;
    font-weight: 600;
    color: var(--vt-c-white);
    margin: 0;
    padding-bottom: 10px;

    @media (max-width: 768px) {
      font-size: 16px;
    }
  }

  &__users-info {
    font-size: 14px;
    color: var(--vt-c-light-gray);

    @media (max-width: 768px) {
      font-size: 12px;
    }
  }

  &__table-wrapper {
    overflow-x: auto;
    margin-bottom: 20px;

    @media (max-width: 768px) {
      overflow: visible;
    }
  }

  &__table {
    width: 100%;
    border-collapse: collapse;
    color: var(--vt-c-white);

    @media (max-width: 768px) {
      display: none;
    }

    th, td {
      padding: 12px;
      text-align: left;
      border-bottom: 1px solid rgba(255, 255, 255, 0.1);
    }

    th {
      font-weight: 600;
      color: var(--vt-c-light-gray);
    }
  }

  &__mobile-cards {
    display: none;

    @media (max-width: 768px) {
      display: block;
      margin-bottom: 20px;
    }
  }

  &__mobile-card {
    background-color: rgba(255, 255, 255, 0.05);
    border-radius: 8px;
    padding: 16px;
    margin-bottom: 16px;

    &:last-child {
      margin-bottom: 0;
    }
  }

  &__mobile-row {
    display: flex;
    justify-content: space-between;
    align-items: flex-start;
    margin-bottom: 8px;
    gap: 8px;

    &:last-child {
      margin-bottom: 0;
    }
  }

  &__mobile-label {
    font-size: 14px;
    color: var(--vt-c-light-gray);
    font-weight: 500;
    flex-shrink: 0;
  }

  &__mobile-value {
    font-size: 14px;
    color: var(--vt-c-white);
    text-align: right;
    word-break: break-word;
  }

  &__mobile-edit {
    flex: 1;
    min-width: 0;
  }

  &__mobile-actions {
    margin-top: 12px;
    padding-top: 12px;
    border-top: 1px solid rgba(255, 255, 255, 0.1);
  }

  &__edit-field {
    width: 100%;
  }

  &__edit-actions {
    display: flex;
    gap: 8px;
    flex-wrap: wrap;
  }

  &__input {
    width: 100%;
    padding: 8px 12px;
    border: 1px solid rgba(255, 255, 255, 0.2);
    border-radius: 4px;
    background-color: rgba(255, 255, 255, 0.1);
    color: var(--vt-c-white);
    font-size: 14px;
    box-sizing: border-box;


    &:focus {
      outline: none;
      border-color: var(--vt-c-light-green);
    }

    &--mobile {
      font-size: 14px;
      padding: 6px 8px;
    }
  }

  &__action-btn {
    padding: 6px 12px;
    border-radius: 4px;
    border: none;
    background-color: var(--vt-c-light-gray);
    color: var(--vt-c-dark-blue-gray);
    font-size: 14px;
    font-weight: 500;
    cursor: pointer;
    margin-right: 8px;
    transition: all 0.2s ease;

    @media (max-width: 768px) {
      margin-right: 0;
      flex: 1;
      min-width: 0;
      font-size: 12px;
      padding: 8px 12px;
    }

    &:hover:not(:disabled) {
      background-color: var(--vt-c-white);
    }

    &:disabled {
      opacity: 0.6;
      cursor: not-allowed;
    }

    &--success {
      background-color: rgba(137, 225, 89, 0.2);
      color: var(--vt-c-light-green);

      &:hover:not(:disabled) {
        background-color: rgba(137, 225, 89, 0.3);
      }
    }
  }

  &__pagination {
    display: flex;
    justify-content: space-between;
    align-items: center;
    padding: 16px 0;
    border-top: 1px solid rgba(255, 255, 255, 0.1);

    @media (max-width: 768px) {
      flex-direction: column;
      gap: 12px;
    }
  }

  &__pagination-btn {
    display: flex;
    align-items: center;
    gap: 8px;
    padding: 8px 16px;
    border: 1px solid rgba(255, 255, 255, 0.2);
    border-radius: 6px;
    background-color: rgba(255, 255, 255, 0.05);
    color: var(--vt-c-white);
    font-size: 14px;
    cursor: pointer;
    transition: all 0.2s ease;

    &:hover:not(:disabled) {
      background-color: rgba(255, 255, 255, 0.1);
    }

    &:disabled {
      opacity: 0.5;
      cursor: not-allowed;
    }

    @media (max-width: 768px) {
      padding: 10px 16px;
    }
  }

  &__pagination-info {
    display: flex;
    align-items: center;
    gap: 16px;

    @media (max-width: 768px) {
      flex-direction: column;
      gap: 8px;
      text-align: center;
    }
  }

  &__pagination-pages {
    font-size: 14px;
    color: var(--vt-c-light-gray);
  }

  &__pagination-select {
    padding: 6px 8px;
    border: 1px solid rgba(255, 255, 255, 0.2);
    border-radius: 4px;
    background-color: rgba(255, 255, 255, 0.1);
    color: var(--vt-c-white);
    font-size: 12px;

    option {
      background-color: var(--vt-c-dark-blue-gray);
      color: var(--vt-c-white);
    }
  }

  &__settings {
    display: grid;
    gap: 16px;

    @media (max-width: 768px) {
      gap: 12px;
    }
  }

  &__setting-item {
    display: flex;
    align-items: center;
    justify-content: space-between;
    padding: 16px;
    background-color: rgba(255, 255, 255, 0.05);
    border-radius: 8px;

    @media (max-width: 768px) {
      padding: 12px;
      flex-wrap: wrap;
      gap: 8px;
    }
  }

  &__setting-info {
    display: flex;
    align-items: center;
    gap: 12px;
    color: var(--vt-c-white);

    @media (max-width: 768px) {
      gap: 8px;
      font-size: 14px;
    }
  }

  &__setting-icon {
    color: var(--vt-c-light-gray);

    @media (max-width: 768px) {
      width: 20px;
      height: 20px;
    }
  }
}

.info-card {
  background-color: var(--vt-c-dark-blue-gray);
  border-radius: 16px;
  padding: 24px;
  display: flex;
  align-items: center;
  gap: 16px;
  box-shadow: 0 4px 6px -1px rgba(0, 0, 0, 0.1);

  &__icon-wrapper {
    width: 48px;
    height: 48px;
    border-radius: 12px;
    background-color: rgba(137, 225, 89, 0.1);
    display: flex;
    align-items: center;
    justify-content: center;
  }

  &__icon {
    color: var(--vt-c-light-green);
  }

  &__details {
    flex: 1;
  }

  &__value {
    font-size: 24px;
    font-weight: 600;
    color: var(--vt-c-white);
    margin: 0 0 4px 0;
  }

  &__label {
    font-size: 14px;
    color: var(--vt-c-light-gray);
    margin: 0;
  }
}

.switch {
  position: relative;
  display: inline-block;
  width: 48px;
  height: 24px;

  @media (max-width: 768px) {
    flex-shrink: 0;
  }

  input {
    opacity: 0;
    width: 0;
    height: 0;
  }

  .slider {
    position: absolute;
    cursor: pointer;
    top: 0;
    left: 0;
    right: 0;
    bottom: 0;
    background-color: rgba(255, 255, 255, 0.1);
    transition: .4s;

    &:before {
      position: absolute;
      content: "";
      height: 18px;
      width: 18px;
      left: 3px;
      bottom: 3px;
      background-color: var(--vt-c-white);
      transition: .4s;
    }
  }

  input:checked + .slider {
    background-color: var(--vt-c-light-green);
  }

  input:checked + .slider:before {
    transform: translateX(24px);
  }

  .slider.round {
    border-radius: 24px;
  }

  .slider.round:before {
    border-radius: 50%;
  }
}

.loading-spinner {
  margin-bottom: 16px;
}

.spinner {
  width: 40px;
  height: 40px;
  border: 3px solid rgba(137, 225, 89, 0.2);
  border-top: 3px solid var(--vt-c-light-green);
  border-radius: 50%;
  animation: spin 1s linear infinite;

  @media (max-width: 768px) {
    width: 32px;
    height: 32px;
    border-width: 2px;
  }
}

@keyframes spin {
  0% { transform: rotate(0deg); }
  100% { transform: rotate(360deg); }
}

.loading-text {
  color: var(--vt-c-light-gray);
  font-size: 16px;
  font-weight: 500;
  margin: 0;
  text-align: center;
  opacity: 0.8;

  @media (max-width: 768px) {
    font-size: 14px;
  }
}

.button-spinner {
  display: inline-block;
  width: 12px;
  height: 12px;
  border: 2px solid rgba(255, 255, 255, 0.3);
  border-top: 2px solid currentColor;
  border-radius: 50%;
  animation: spin 0.8s linear infinite;
  margin-right: 8px;
  vertical-align: middle;
}
</style>
