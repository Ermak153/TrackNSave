<template>
  <div class="contacts">
    <div class="contacts__container">
      <div class="contacts__header">
        <h1 class="contacts__title">Контакты</h1>
        <p class="contacts__subtitle">Свяжитесь с нами, если у вас есть вопросы или предложения</p>
      </div>

      <div class="contacts__cards">
        <div class="info-card">
          <div class="info-card__icon-wrapper">
            <svg class="info-card__icon" xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
              <path d="M22 16.92v3a2 2 0 0 1-2.18 2 19.79 19.79 0 0 1-8.63-3.07 19.5 19.5 0 0 1-6-6 19.79 19.79 0 0 1-3.07-8.67A2 2 0 0 1 4.11 2h3a2 2 0 0 1 2 1.72 12.84 12.84 0 0 0 .7 2.81 2 2 0 0 1-.45 2.11L8.09 9.91a16 16 0 0 0 6 6l1.27-1.27a2 2 0 0 1 2.11-.45 12.84 12.84 0 0 0 2.81.7A2 2 0 0 1 22 16.92z"/>
            </svg>
          </div>
          <div class="info-card__details">
            <h3 class="info-card__value">+7 (999) 123-45-67</h3>
            <p class="info-card__label">Телефон</p>
          </div>
        </div>

        <div class="info-card">
          <div class="info-card__icon-wrapper">
            <svg class="info-card__icon" xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
              <path d="M4 4h16c1.1 0 2 .9 2 2v12c0 1.1-.9 2-2 2H4c-1.1 0-2-.9-2-2V6c0-1.1.9-2 2-2z"/>
              <polyline points="22,6 12,13 2,6"/>
            </svg>
          </div>
          <div class="info-card__details">
            <h3 class="info-card__value">support@tracknsave.ru</h3>
            <p class="info-card__label">Email</p>
          </div>
        </div>

        <div class="info-card">
          <div class="info-card__icon-wrapper">
            <svg class="info-card__icon" xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
              <path d="M21 10c0 7-9 13-9 13s-9-6-9-13a9 9 0 0 1 18 0z"/>
              <circle cx="12" cy="10" r="3"/>
            </svg>
          </div>
          <div class="info-card__details">
            <h3 class="info-card__value">Москва, ул. Примерная, 123</h3>
            <p class="info-card__label">Адрес</p>
          </div>
        </div>
      </div>

      <div class="contacts__sections">
        <div class="contacts__section">
          <h2 class="contacts__section-title">Социальные сети</h2>
          <div class="contacts__social">
            <a href="#" class="contacts__social-link">
              <svg class="contacts__social-icon" xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
                <path d="M18 2h-3a5 5 0 0 0-5 5v3H7v4h3v8h4v-8h3l1-4h-4V7a1 1 0 0 1 1-1h3z"/>
              </svg>
              <span>Facebook</span>
            </a>
            <a href="#" class="contacts__social-link">
              <svg class="contacts__social-icon" xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
                <path d="M23 3a10.9 10.9 0 0 1-3.14 1.53 4.48 4.48 0 0 0-7.86 3v1A10.66 10.66 0 0 1 3 4s-4 9 5 13a11.64 11.64 0 0 1-7 2c9 5 20 0 20-11.5a4.5 4.5 0 0 0-.08-.83A7.72 7.72 0 0 0 23 3z"/>
              </svg>
              <span>Twitter</span>
            </a>
            <a href="#" class="contacts__social-link">
              <svg class="contacts__social-icon" xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
                <rect x="2" y="2" width="20" height="20" rx="5" ry="5"/>
                <path d="M16 11.37A4 4 0 1 1 12.63 8 4 4 0 0 1 16 11.37z"/>
                <line x1="17.5" y1="6.5" x2="17.51" y2="6.5"/>
              </svg>
              <span>Instagram</span>
            </a>
          </div>
        </div>

        <div class="contacts__section">
          <h2 class="contacts__section-title">Форма обратной связи</h2>
          <form class="contacts__form">
            <div class="contacts__form-group">
              <label class="contacts__form-label">Имя</label>
              <input type="text" class="contacts__form-input" placeholder="Ваше имя">
            </div>
            <div class="contacts__form-group">
              <label class="contacts__form-label">Email</label>
              <input type="email" class="contacts__form-input" placeholder="Ваш email">
            </div>
            <div class="contacts__form-group">
              <label class="contacts__form-label">Сообщение</label>
              <textarea class="contacts__form-textarea" placeholder="Ваше сообщение"></textarea>
            </div>
            <button type="submit" class="contacts__form-button">Отправить</button>
          </form>
        </div>
      </div>
    </div>
  </div>
</template>

<script lang="ts" setup>

</script>

<style lang="scss" scoped>
.contacts {
  &__container {
    width: 100%;
    max-width: 1280px;
    margin: 0 auto;
    box-sizing: border-box;
  }

  &__header {
    margin-bottom: 32px;
    background-color: var(--vt-c-dark-blue-gray);
    border-radius: 16px;
    padding: 24px;
    box-shadow: 0 4px 6px -1px rgba(0, 0, 0, 0.1);
  }

  &__title {
    font-size: 32px;
    font-weight: 600;
    color: var(--vt-c-white);
    margin: 0 0 8px 0;
  }

  &__subtitle {
    font-size: 16px;
    color: var(--vt-c-light-gray);
    margin: 0;
  }

  &__cards {
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
    grid-template-columns: repeat(2, 1fr);
    gap: 32px;

    @media (max-width: 768px) {
      grid-template-columns: 1fr;
    }
  }

  &__section {
    background-color: var(--vt-c-dark-blue-gray);
    border-radius: 16px;
    padding: 24px;
    box-shadow: 0 4px 6px -1px rgba(0, 0, 0, 0.1);
  }

  &__section-title {
    font-size: 24px;
    font-weight: 600;
    color: var(--vt-c-white);
    margin: 0 0 24px 0;
  }

  &__social {
    display: flex;
    flex-direction: column;
    gap: 16px;
  }

  &__social-link {
    display: flex;
    align-items: center;
    gap: 12px;
    color: var(--vt-c-white);
    text-decoration: none;
    transition: color 0.2s;

    &:hover {
      color: var(--primary-green);
    }
  }

  &__social-icon {
    color: var(--primary-green);
    width: 24px;
    height: 24px;
  }

  &__form {
    display: flex;
    flex-direction: column;
    gap: 16px;
  }

  &__form-group {
    display: flex;
    flex-direction: column;
    gap: 8px;
  }

  &__form-label {
    font-size: 16px;
    color: var(--vt-c-white);
  }

  &__form-input,
  &__form-textarea {
    padding: 12px;
    border-radius: 8px;
    border: 1px solid var(--vt-c-blue-gray);
    background-color: var(--vt-c-light-background);
    color: var(--vt-c-white);
    font-size: 16px;
    transition: border-color 0.2s;

    &:focus {
      outline: none;
      border-color: var(--primary-green);
    }
  }

  &__form-textarea {
    min-height: 120px;
    resize: vertical;
  }

  &__form-button {
    padding: 12px 24px;
    background-color: var(--primary-green);
    color: var(--vt-c-dark-blue-gray);
    border: none;
    border-radius: 8px;
    font-size: 16px;
    font-weight: 600;
    cursor: pointer;
    transition: background-color 0.2s;

    &:hover {
      background-color: var(--vt-c-dark-green);
    }
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
