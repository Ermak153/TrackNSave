<template>
  <div class="contacts">
    <div class="contacts__container">
      <div class="contacts__header">
        <h1 class="contacts__title">О проекте</h1>
        <p class="contacts__subtitle">Дипломный проект по разработке веб-приложения для учета чеков</p>
      </div>

      <div class="contacts__sections">
        <div class="contacts__section contacts__section--main">
          <h2 class="contacts__section-title">TrackNSave</h2>
          <div class="contacts__description">
            <p class="contacts__text">
              TrackNSave — это веб-приложение для учета и анализа чеков, разработанное в рамках дипломного проекта.
              Приложение позволяет пользователям сканировать QR-коды с чеков, автоматически получать данные о покупках
              и вести учет своих расходов.
            </p>
            <p class="contacts__text">
              Проект реализован с использованием современных технологий: ASP.NET Core для backend,
              Vue.js 3 с TypeScript для frontend, и PostgreSQL в качестве базы данных.
            </p>
          </div>
        </div>

        <div class="contacts__section">
          <h2 class="contacts__section-title">Связаться с разработчиком</h2>
          <div class="contacts__social">
            <a href="https://t.me/neko153" target="_blank" class="contacts__social-link">
              <div class="contacts__social-icon-wrapper">
                <svg class="contacts__social-icon" xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
                  <path d="M21.198 2.433a2.242 2.242 0 0 0-1.022.215l-8.609 3.33c-2.068.8-4.133 1.598-5.724 2.21a205.66 205.66 0 0 1-2.849 1.09c-.42.15-.593.27-.593.442 0 .173.173.293.593.442.42.15 1.537.593 2.849 1.09 1.591.612 3.656 1.41 5.724 2.21l8.609 3.33c.35.135.672.215 1.022.215.7 0 1.278-.578 1.278-1.278V3.71c0-.7-.578-1.278-1.278-1.278z"/>
                  <path d="m10.5 11.5 5.5-5.5"/>
                  <path d="m16 16-5.5-5.5"/>
                </svg>
              </div>
              <div class="contacts__social-info">
                <span class="contacts__social-name">Telegram</span>
                <span class="contacts__social-description">Быстрая связь и вопросы</span>
              </div>
            </a>

            <a href="https://github.com/Ermak153/TrackNSave/" target="_blank" class="contacts__social-link">
              <div class="contacts__social-icon-wrapper">
                <svg class="contacts__social-icon" xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
                  <path d="M9 19c-5 1.5-5-2.5-7-3m14 6v-3.87a3.37 3.37 0 0 0-.94-2.61c3.14-.35 6.44-1.54 6.44-7A5.44 5.44 0 0 0 20 4.77 5.07 5.07 0 0 0 19.91 1S18.73.65 16 2.48a13.38 13.38 0 0 0-7 0C6.27.65 5.09 1 5.09 1A5.07 5.07 0 0 0 5 4.77a5.44 5.44 0 0 0-1.5 3.78c0 5.42 3.3 6.61 6.44 7A3.37 3.37 0 0 0 9 18.13V22"/>
                </svg>
              </div>
              <div class="contacts__social-info">
                <span class="contacts__social-name">GitHub</span>
                <span class="contacts__social-description">Исходный код проекта</span>
              </div>
            </a>
          </div>
        </div>
      </div>

      <div class="contacts__tech-section">
        <h2 class="contacts__section-title">Технологии</h2>
        <div class="contacts__tech-grid">
          <div class="tech-card">
            <div class="tech-card__icon-wrapper">
              <svg class="tech-card__icon" xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
                <polyline points="16 18 22 12 16 6"/>
                <polyline points="8 6 2 12 8 18"/>
              </svg>
            </div>
            <div class="tech-card__details">
              <h3 class="tech-card__name">Frontend</h3>
              <p class="tech-card__description">Vue.js 3, TypeScript, SCSS</p>
            </div>
          </div>

          <div class="tech-card">
            <div class="tech-card__icon-wrapper">
              <svg class="tech-card__icon" xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
                <rect x="2" y="3" width="20" height="14" rx="2" ry="2"/>
                <line x1="8" y1="21" x2="16" y2="21"/>
                <line x1="12" y1="17" x2="12" y2="21"/>
              </svg>
            </div>
            <div class="tech-card__details">
              <h3 class="tech-card__name">Backend</h3>
              <p class="tech-card__description">ASP.NET Core, Entity Framework</p>
            </div>
          </div>

          <div class="tech-card">
            <div class="tech-card__icon-wrapper">
              <svg class="tech-card__icon" xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
                <ellipse cx="12" cy="5" rx="9" ry="3"/>
                <path d="M21 12c0 1.66-4 3-9 3s-9-1.34-9-3"/>
                <path d="M3 5v14c0 1.66 4 3 9 3s9-1.34 9-3V5"/>
              </svg>
            </div>
            <div class="tech-card__details">
              <h3 class="tech-card__name">База данных</h3>
              <p class="tech-card__description">PostgreSQL</p>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<style lang="scss" scoped>
.contacts {
  padding: 0 20px;

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

    @media (max-width: 768px) {
      font-size: 28px;
    }
  }

  &__subtitle {
    font-size: 16px;
    color: var(--vt-c-light-gray);
    margin: 0;

    @media (max-width: 768px) {
      font-size: 14px;
    }
  }

  &__sections {
    display: grid;
    grid-template-columns: 2fr 1fr;
    gap: 32px;
    margin-bottom: 32px;

    @media (max-width: 768px) {
      grid-template-columns: 1fr;
      gap: 20px;
    }
  }

  &__section {
    background-color: var(--vt-c-dark-blue-gray);
    border-radius: 16px;
    padding: 24px;
    box-shadow: 0 4px 6px -1px rgba(0, 0, 0, 0.1);

    &--main {
      @media (max-width: 768px) {
        order: 1;
      }
    }

    @media (max-width: 768px) {
      padding: 20px;
    }
  }

  &__section-title {
    font-size: 24px;
    font-weight: 600;
    color: var(--vt-c-white);
    margin: 0 0 20px 0;

    @media (max-width: 768px) {
      font-size: 20px;
      margin-bottom: 16px;
    }
  }

  &__description {
    display: flex;
    flex-direction: column;
    gap: 16px;
  }

  &__text {
    font-size: 16px;
    line-height: 1.6;
    color: var(--vt-c-light-gray);
    margin: 0;

    @media (max-width: 768px) {
      font-size: 14px;
    }
  }

  &__social {
    display: flex;
    flex-direction: column;
    gap: 16px;
  }

  &__social-link {
    display: flex;
    align-items: center;
    gap: 16px;
    padding: 16px;
    background-color: rgba(255, 255, 255, 0.05);
    border-radius: 12px;
    color: var(--vt-c-white);
    text-decoration: none;
    transition: all 0.2s ease;

    &:hover {
      background-color: rgba(137, 225, 89, 0.1);
      transform: translateY(-2px);
    }

    @media (max-width: 768px) {
      padding: 12px;
      gap: 12px;
    }
  }

  &__social-icon-wrapper {
    display: flex;
    align-items: center;
    justify-content: center;
    width: 48px;
    height: 48px;
    background-color: rgba(137, 225, 89, 0.2);
    border-radius: 12px;
    flex-shrink: 0;

    @media (max-width: 768px) {
      width: 40px;
      height: 40px;
    }
  }

  &__social-icon {
    color: var(--vt-c-light-green);
    width: 24px;
    height: 24px;

    @media (max-width: 768px) {
      width: 20px;
      height: 20px;
    }
  }

  &__social-info {
    display: flex;
    flex-direction: column;
    gap: 4px;
  }

  &__social-name {
    font-size: 18px;
    font-weight: 600;
    color: var(--vt-c-white);

    @media (max-width: 768px) {
      font-size: 16px;
    }
  }

  &__social-description {
    font-size: 14px;
    color: var(--vt-c-light-gray);

    @media (max-width: 768px) {
      font-size: 12px;
    }
  }

  &__tech-section {
    background-color: var(--vt-c-dark-blue-gray);
    border-radius: 16px;
    padding: 24px;
    box-shadow: 0 4px 6px -1px rgba(0, 0, 0, 0.1);

    @media (max-width: 768px) {
      padding: 20px;
    }
  }

  &__tech-grid {
    display: grid;
    grid-template-columns: repeat(3, 1fr);
    gap: 20px;

    @media (max-width: 768px) {
      grid-template-columns: 1fr;
      gap: 16px;
    }
  }
}

.tech-card {
  display: flex;
  align-items: center;
  gap: 16px;
  padding: 20px;
  background-color: rgba(255, 255, 255, 0.05);
  border-radius: 12px;
  transition: all 0.2s ease;

  &:hover {
    background-color: rgba(255, 255, 255, 0.08);
    transform: translateY(-2px);
  }

  @media (max-width: 768px) {
    padding: 16px;
    gap: 12px;
  }

  &__icon-wrapper {
    display: flex;
    align-items: center;
    justify-content: center;
    width: 48px;
    height: 48px;
    background-color: rgba(137, 225, 89, 0.2);
    border-radius: 12px;
    flex-shrink: 0;

    @media (max-width: 768px) {
      width: 40px;
      height: 40px;
    }
  }

  &__icon {
    color: var(--vt-c-light-green);
    width: 24px;
    height: 24px;

    @media (max-width: 768px) {
      width: 20px;
      height: 20px;
    }
  }

  &__details {
    flex: 1;
  }

  &__name {
    font-size: 18px;
    font-weight: 600;
    color: var(--vt-c-white);
    margin: 0 0 4px 0;

    @media (max-width: 768px) {
      font-size: 16px;
    }
  }

  &__description {
    font-size: 14px;
    color: var(--vt-c-light-gray);
    margin: 0;

    @media (max-width: 768px) {
      font-size: 12px;
    }
  }
}
</style>
