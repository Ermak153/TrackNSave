<template>
  <div class="home">
    <div class="home__container">
      <div class="home__hero">
        <div class="home__hero-content">
          <h1 class="home__title">TrackNSave</h1>
          <p class="home__subtitle">Умное приложение для учета расходов</p>
          <div class="home__buttons">
            <router-link class="home__button home__button--primary" v-if="!isAuthenticated" to="/login">Начать использовать</router-link>
            <router-link class="home__button home__button--primary" v-if="isAuthenticated" to="/receipt">Начать использовать</router-link>
            <a class="home__button home__button--secondary" href="#features">Узнать больше</a>
          </div>
        </div>

      </div>

      <div class="home__features" id="features">
        <div class="info-card">
          <div class="info-card__icon-wrapper">
            <svg class="info-card__icon" xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
              <path d="M12 2v20M17 5H9.5a3.5 3.5 0 0 0 0 7h5a3.5 3.5 0 0 1 0 7H6"/>
            </svg>
          </div>
          <div class="info-card__details">
            <h3 class="info-card__value">Учет расходов</h3>
            <p class="info-card__label">Отслеживайте все свои покупки в одном месте</p>
          </div>
        </div>

        <div class="info-card">
          <div class="info-card__icon-wrapper">
            <svg class="info-card__icon" xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
              <path d="M3 3v18h18"/>
              <path d="M18 17v4"/>
              <path d="M13 13v8"/>
              <path d="M8 9v12"/>
            </svg>
          </div>
          <div class="info-card__details">
            <h3 class="info-card__value">Аналитика</h3>
            <p class="info-card__label">Наглядные графики динамики цен на отдельные товары</p>
          </div>
        </div>

        <div class="info-card">
          <div class="info-card__icon-wrapper">
            <svg class="info-card__icon" xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
              <path d="M12 2v20M17 5H9.5a3.5 3.5 0 0 0 0 7h5a3.5 3.5 0 0 1 0 7H6"/>
            </svg>
          </div>
          <div class="info-card__details">
            <h3 class="info-card__value">Экономия</h3>
            <p class="info-card__label">Помогаем находить возможности для экономии</p>
          </div>
        </div>
      </div>

      <div class="home__sections">
        <div class="home__section">
          <h2 class="home__section-title">Как это работает?</h2>
          <div class="home__steps">
            <div class="home__step">
              <div class="home__step-number">1</div>
              <div class="home__step-content">
                <h3 class="home__step-title">Добавьте чек</h3>
                <p class="home__step-description">Просто отсканируйте QR-код с чека или введите данные вручную</p>
              </div>
            </div>
            <div class="home__step">
              <div class="home__step-number">2</div>
              <div class="home__step-content">
                <h3 class="home__step-title">Анализируйте</h3>
                <p class="home__step-description">Получайте подробную статистику по вашим расходам</p>
              </div>
            </div>
            <div class="home__step">
              <div class="home__step-number">3</div>
              <div class="home__step-content">
                <h3 class="home__step-title">Экономьте</h3>
                <p class="home__step-description">Используйте полученные данные для оптимизации расходов</p>
              </div>
            </div>
          </div>
        </div>

        <div class="home__section">
          <h2 class="home__section-title">Преимущества</h2>
          <div class="home__benefits">
            <div class="home__benefit">
              <svg class="home__benefit-icon" xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
                <path d="M22 11.08V12a10 10 0 1 1-5.93-9.14"/>
                <polyline points="22 4 12 14.01 9 11.01"/>
              </svg>
              <span>Простой и удобный интерфейс</span>
            </div>
            <div class="home__benefit">
              <svg class="home__benefit-icon" xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
                <path d="M22 11.08V12a10 10 0 1 1-5.93-9.14"/>
                <polyline points="22 4 12 14.01 9 11.01"/>
              </svg>
              <span>Быстрый ввод чеков через QR-код</span>
            </div>
            <div class="home__benefit">
              <svg class="home__benefit-icon" xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
                <path d="M22 11.08V12a10 10 0 1 1-5.93-9.14"/>
                <polyline points="22 4 12 14.01 9 11.01"/>
              </svg>
              <span>Автоматическая категоризация товаров</span>
            </div>
            <div class="home__benefit">
              <svg class="home__benefit-icon" xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
                <path d="M22 11.08V12a10 10 0 1 1-5.93-9.14"/>
                <polyline points="22 4 12 14.01 9 11.01"/>
              </svg>
              <span>Формирование чеков в PDF</span>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
  import { onMounted } from 'vue';
  import { useAuth } from '@/composables/useAuth';
  const { isAuthenticated, checkAuthStatus } = useAuth();

  onMounted(async () => {
    await checkAuthStatus()
  })
</script>

<style lang="scss" scoped>
  .home {
    padding: 0 20px;

    &__container {
      width: 100%;
      max-width: 1280px;
      margin: 0 auto;
      box-sizing: border-box;
    }

    &__hero {
      display: flex;
      align-items: center;
      justify-content: space-between;
      margin-bottom: 64px;
      background-color: var(--vt-c-dark-blue-gray);
      border-radius: 16px;
      padding: 48px;
      box-shadow: 0 4px 6px -1px rgba(0, 0, 0, 0.1);

      @media (max-width: 768px) {
        flex-direction: column;
        text-align: center;
        padding: 32px;
      }
    }

    &__hero-content {
      flex: 1;
      margin-right: 48px;

      @media (max-width: 768px) {
        margin-right: 0;
        margin-bottom: 32px;
      }
    }

    &__title {
      font-size: 48px;
      font-weight: 700;
      color: var(--vt-c-white);
      margin: 0 0 16px 0;
    }

    &__subtitle {
      font-size: 20px;
      color: var(--vt-c-light-gray);
      margin: 0 0 32px 0;
    }

    &__buttons {
      display: flex;
      gap: 16px;

      @media (max-width: 768px) {
        justify-content: center;
      }
    }

    &__button {
      padding: 12px 24px;
      border-radius: 8px;
      font-size: 18px;
      font-weight: 700;
      cursor: pointer;
      transition: all 0.2s;
      text-decoration: none;

      &--primary {
        background-color: var(--primary-green);
        color: var(--vt-c-dark-blue-gray);
        border: 2px solid var(--primary-green);

        @media (hover: hover) and (pointer: fine) {
          &:hover {
            background: none;
            color: var(--primary-green);
          }
        }
      }

      &--secondary {
        background: none;
        color: var(--vt-c-white);
        border: 2px solid var(--vt-c-white);

        @media (hover: hover) and (pointer: fine) {
          &:hover {
            background: var(--vt-c-white);
            color: var(--vt-c-dark-blue-gray);
          }
        }
      }
    }

    &__hero-image {
      flex: 1;
      display: flex;
      justify-content: center;
      align-items: center;
    }

    &__image {
      color: var(--primary-green);
      max-width: 100%;
      height: auto;
    }

    &__features {
      display: grid;
      grid-template-columns: repeat(3, 1fr);
      gap: 20px;
      margin-bottom: 64px;

      @media (max-width: 768px) {
        grid-template-columns: 1fr;
      }
    }

    &__sections {
      display: grid;
      grid-template-columns: repeat(2, 1fr);
      gap: 32px;

      @media (max-width: 768px) {
        grid-template-columns: 1fr;
      }
    }

    &__section {
      background-color: var(--vt-c-dark-blue-gray);
      border-radius: 16px;
      padding: 32px;
      box-shadow: 0 4px 6px -1px rgba(0, 0, 0, 0.1);
    }

    &__section-title {
      font-size: 24px;
      font-weight: 600;
      color: var(--vt-c-white);
      margin: 0 0 24px 0;
    }

    &__steps {
      display: flex;
      flex-direction: column;
      gap: 24px;
    }

    &__step {
      display: flex;
      align-items: flex-start;
      gap: 16px;
    }

    &__step-number {
      display: flex;
      align-items: center;
      justify-content: center;
      width: 32px;
      height: 32px;
      background-color: var(--primary-green);
      color: var(--vt-c-dark-blue-gray);
      border-radius: 50%;
      font-size: 16px;
      font-weight: 600;
      flex-shrink: 0;
    }

    &__step-content {
      flex: 1;
    }

    &__step-title {
      font-size: 18px;
      font-weight: 600;
      color: var(--vt-c-white);
      margin: 0 0 8px 0;
    }

    &__step-description {
      font-size: 14px;
      color: var(--vt-c-light-gray);
      margin: 0;
    }

    &__benefits {
      display: flex;
      flex-direction: column;
      gap: 16px;
    }

    &__benefit {
      display: flex;
      align-items: center;
      gap: 12px;
      color: var(--vt-c-white);
      font-size: 16px;
    }

    &__benefit-icon {
      color: var(--primary-green);
      width: 20px;
      height: 20px;
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
</style>
