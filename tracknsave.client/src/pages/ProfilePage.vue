<template>
  <div class="profile">
    <div class="profile__container">
      <div class="profile__header">
        <div class="profile__avatar">
          <div class="profile__avatar-inner">
            <span class="profile__avatar-text">ИИ</span>
          </div>
        </div>
        <div class="profile__info">
          <h1 class="profile__name">123123</h1>
          <p class="profile__email">123123@yandex.ru</p>
        </div>
      </div>

      <div class="profile__stats">
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
            <h3 class="info-card__value">127</h3>
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
            <h3 class="info-card__value">45,678 ₽</h3>
            <p class="info-card__label">Общая сумма</p>
          </div>
        </div>

        <div class="info-card">
          <div class="info-card__icon-wrapper">
            <svg class="info-card__icon" xmlns="http://www.w3.org/2000/svg" width="24" height="24"
                viewBox="0 0 24 24" fill="none" stroke="currentColor"
                stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
              <polygon points="12 2 15 8.5 22 9.3 17 14 18.5 21 12 17.5 5.5 21 7 14 2 9.3 9 8.5 12 2"/>
            </svg>
          </div>
          <div class="info-card__details">
            <h3 class="info-card__value">Продукты питания</h3>
            <p class="info-card__label">Популярная категория</p>
          </div>
        </div>
      </div>

      <div class="profile__sections">
        <div class="profile__section">
          <h2 class="profile__section-title">Личная информация</h2>
          <div class="profile__info-grid">
            <div class="profile__info-item">
              <label class="profile__info-label">Логин</label>
              <p class="profile__info-value">123123</p>
            </div>
            <div class="profile__info-item">
              <label class="profile__info-label">В приложении</label>
              <p class="profile__info-value">1 год 3 мес</p>
            </div>
            <div class="profile__info-item">
              <label class="profile__info-label">Email</label>
              <p class="profile__info-value">123123@yandex.ru</p>
            </div>
          </div>
        </div>

        <div class="profile__section">
          <h2 class="profile__section-title">Настройки</h2>
          <div class="profile__settings">
            <div class="profile__setting-item">
              <div class="profile__setting-info">
                <svg class="profile__setting-icon" xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
                  <path d="M18 8A6 6 0 0 0 6 8c0 7-3 9-3 9h18s-3-2-3-9"></path>
                  <path d="M13.73 21a2 2 0 0 1-3.46 0"></path>
                </svg>
                <span>Уведомления</span>
              </div>
              <label class="switch">
                <input type="checkbox" checked>
                <span class="slider round"></span>
              </label>
            </div>
            <div class="profile__setting-item">
              <div class="profile__setting-info">
                <svg class="profile__setting-icon" xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
                  <path d="M21 12.79A9 9 0 1 1 11.21 3 7 7 0 0 0 21 12.79z"></path>
                </svg>
                <span>Темная тема</span>
              </div>
              <label class="switch">
                <input type="checkbox">
                <span class="slider round"></span>
              </label>
            </div>
            <div class="profile__setting-item">
              <div class="profile__setting-info">
                <svg class="profile__setting-icon" xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
                  <circle cx="12" cy="12" r="10"></circle>
                  <line x1="2" y1="12" x2="22" y2="12"></line>
                  <path d="M12 2a15.3 15.3 0 0 1 4 10 15.3 15.3 0 0 1-4 10 15.3 15.3 0 0 1-4-10 15.3 15.3 0 0 1 4-10z"></path>
                </svg>
                <span>Язык</span>
              </div>
              <span class="profile__setting-value">Русский</span>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
// Пока что пустой, так как используем тестовые данные
</script>

<style lang="scss" scoped>
.profile {
  &__container {
    width: 100%;
    max-width: 1280px;
    margin: 0 auto;
    box-sizing: border-box;
  }

  &__header {
    display: flex;
    align-items: center;
    margin-bottom: 32px;
    background-color: var(--vt-c-dark-blue-gray);
    border-radius: 16px;
    padding: 24px;
    box-shadow: 0 4px 6px -1px rgba(0, 0, 0, 0.1);

    @media (max-width: 768px) {
      flex-direction: column;
      text-align: center;
    }
  }

  &__avatar {
    margin-right: 24px;

    @media (max-width: 768px) {
      margin-right: 0;
      margin-bottom: 16px;
    }
  }

  &__avatar-inner {
    width: 100px;
    height: 100px;
    border-radius: 50%;
    background: var(--primary-green);
    display: flex;
    align-items: center;
    justify-content: center;
  }

  &__avatar-text {
    color: var(--vt-c-dark-blue-gray);
    font-size: 32px;
    font-weight: bold;
  }

  &__info {
    flex: 1;
  }

  &__name {
    font-size: 24px;
    font-weight: 600;
    color: var(--vt-c-white);
    margin: 0 0 8px 0;
  }

  &__email {
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

  &__info-grid {
    display: grid;
    grid-template-columns: repeat(2, 1fr);
    gap: 20px;

    @media (max-width: 768px) {
      grid-template-columns: 1fr;
    }
  }

  &__info-item {
    label {
      display: block;
      font-size: 14px;
      color: var(--vt-c-light-gray);
      margin-bottom: 8px;
    }

    p {
      font-size: 16px;
      color: var(--vt-c-white);
      margin: 0;
    }
  }

  &__settings {
    display: flex;
    flex-direction: column;
    gap: 16px;
  }

  &__setting-item {
    display: flex;
    align-items: center;
    justify-content: space-between;
    padding: 12px 0;
  }

  &__setting-info {
    display: flex;
    align-items: center;
    gap: 12px;
    color: var(--vt-c-white);
  }

  &__setting-icon {
    color: var(--primary-green);
    width: 20px;
    height: 20px;
  }

  &__setting-value {
    color: var(--vt-c-light-gray);
  }
}

.info-card {
  display: flex;
  align-items: center;
  background-color: var(--vt-c-dark-blue-gray);
  box-shadow: 0 4px 6px -1px rgba(0, 0, 0, 0.1);
  border-radius: 16px;
  padding: 20px;

  &__icon-wrapper {
    display: flex;
    align-items: center;
    justify-content: center;
    width: 48px;
    height: 48px;
    background-color: rgba(137, 225, 89, 0.2);
    border-radius: 12px;
    margin-right: 16px;
  }

  &__icon {
    color: var(--primary-green);
  }

  &__details {
    flex: 1;
  }

  &__value {
    font-size: 24px;
    font-weight: 700;
    color: var(--vt-c-white);
    margin: 0 0 4px 0;
  }

  &__label {
    font-size: 16px;
    color: var(--vt-c-light-gray);
    margin: 0;
  }
}

.switch {
  position: relative;
  display: inline-block;
  width: 50px;
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
    background-color: var(--vt-c-light-background);
    transition: .4s;
    border: 1px solid var(--vt-c-blue-gray);

    &:before {
      position: absolute;
      content: "";
      height: 16px;
      width: 16px;
      left: 4px;
      bottom: 3px;
      background-color: var(--vt-c-blue-gray);
      transition: .4s;
    }

    &.round {
      border-radius: 24px;

      &:before {
        border-radius: 50%;
      }
    }
  }

  input:checked + .slider {
    background-color: var(--primary-green);
    border-color: var(--primary-green);

    &:before {
      transform: translateX(26px);
      background-color: var(--vt-c-white);
    }
  }
}
</style>
