<template>
  <div class="admin">
    <div class="admin__container">
      <div class="admin__header">
        <h1 class="admin__title">Панель администратора</h1>
        <p class="admin__subtitle">Управление пользователями и системой</p>
      </div>

      <div class="admin__stats">
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
            <h3 class="info-card__value">1,234</h3>
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
            <h3 class="info-card__value">45,678</h3>
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
            <h3 class="info-card__value">1,234,567 ₽</h3>
            <p class="info-card__label">Общая сумма</p>
          </div>
        </div>
      </div>

      <div class="admin__sections">
        <div class="admin__section">
          <h2 class="admin__section-title">Управление пользователями</h2>
          <div class="admin__table-wrapper">
            <table class="admin__table">
              <thead>
                <tr>
                  <th>ID</th>
                  <th>Имя пользователя</th>
                  <th>Email</th>
                  <th>Статус</th>
                  <th>Действия</th>
                </tr>
              </thead>
              <tbody>
                <tr>
                  <td>1</td>
                  <td>user123</td>
                  <td>user123@example.com</td>
                  <td><span class="admin__status admin__status--active">Активен</span></td>
                  <td>
                    <button class="admin__action-btn">Редактировать</button>
                    <button class="admin__action-btn admin__action-btn--danger">Заблокировать</button>
                  </td>
                </tr>
                <tr>
                  <td>2</td>
                  <td>user456</td>
                  <td>user456@example.com</td>
                  <td><span class="admin__status admin__status--blocked">Заблокирован</span></td>
                  <td>
                    <button class="admin__action-btn">Редактировать</button>
                    <button class="admin__action-btn admin__action-btn--success">Разблокировать</button>
                  </td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>

        <div class="admin__section">
          <h2 class="admin__section-title">Системные настройки</h2>
          <div class="admin__settings">
            <div class="admin__setting-item">
              <div class="admin__setting-info">
                <svg class="admin__setting-icon" xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
                  <path d="M18 8A6 6 0 0 0 6 8c0 7-3 9-3 9h18s-3-2-3-9"></path>
                  <path d="M13.73 21a2 2 0 0 1-3.46 0"></path>
                </svg>
                <span>Системные уведомления</span>
              </div>
              <label class="switch">
                <input type="checkbox" checked>
                <span class="slider round"></span>
              </label>
            </div>
            <div class="admin__setting-item">
              <div class="admin__setting-info">
                <svg class="admin__setting-icon" xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
                  <path d="M21 12.79A9 9 0 1 1 11.21 3 7 7 0 0 0 21 12.79z"></path>
                </svg>
                <span>Режим обслуживания</span>
              </div>
              <label class="switch">
                <input type="checkbox">
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

</script>

<style lang="scss" scoped>
.admin {
  &__container {
    width: 100%;
    max-width: 1280px;
    margin: 0 auto;
    box-sizing: border-box;
    padding: 24px;
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
  }

  &__section-title {
    font-size: 18px;
    font-weight: 600;
    color: var(--vt-c-white);
    margin: 0 0 20px 0;
  }

  &__table-wrapper {
    overflow-x: auto;
  }

  &__table {
    width: 100%;
    border-collapse: collapse;
    color: var(--vt-c-white);

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

  &__status {
    display: inline-block;
    padding: 4px 8px;
    border-radius: 4px;
    font-size: 14px;
    font-weight: 500;

    &--active {
      background-color: rgba(137, 225, 89, 0.2);
      color: var(--vt-c-light-green);
    }

    &--blocked {
      background-color: rgba(255, 77, 77, 0.2);
      color: var(--vt-c-light-red);
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

    &:hover {
      background-color: var(--vt-c-white);
    }

    &--danger {
      background-color: rgba(255, 77, 77, 0.2);
      color: var(--vt-c-light-red);

      &:hover {
        background-color: rgba(255, 77, 77, 0.3);
      }
    }

    &--success {
      background-color: rgba(137, 225, 89, 0.2);
      color: var(--vt-c-light-green);

      &:hover {
        background-color: rgba(137, 225, 89, 0.3);
      }
    }
  }

  &__settings {
    display: grid;
    gap: 16px;
  }

  &__setting-item {
    display: flex;
    align-items: center;
    justify-content: space-between;
    padding: 16px;
    background-color: rgba(255, 255, 255, 0.05);
    border-radius: 8px;
  }

  &__setting-info {
    display: flex;
    align-items: center;
    gap: 12px;
    color: var(--vt-c-white);
  }

  &__setting-icon {
    color: var(--vt-c-light-gray);
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
</style>
