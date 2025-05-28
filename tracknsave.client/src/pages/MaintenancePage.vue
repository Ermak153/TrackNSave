<template>
  <div class="maintenance-page">
    <div class="maintenance-content">
      <div class="maintenance-icon">
        <svg xmlns="http://www.w3.org/2000/svg" width="64" height="64" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
          <path d="M14.7 6.3a1 1 0 0 0 0 1.4l1.6 1.6a1 1 0 0 0 1.4 0l3.77-3.77a6 6 0 0 1-7.94 7.94l-6.91 6.91a2.12 2.12 0 0 1-3-3l6.91-6.91a6 6 0 0 1 7.94-7.94l-3.76 3.76z"></path>
        </svg>
      </div>

      <h1 class="maintenance-title">Техническое обслуживание</h1>

      <p class="maintenance-message">
        Сайт временно недоступен в связи с проведением технических работ.
      </p>

      <p class="maintenance-submessage">
        Приносим извинения за неудобства. Пожалуйста, попробуйте зайти позже.
      </p>

      <div class="maintenance-info">
        <div class="maintenance-info__item">
          <svg class="maintenance-info__icon" xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
            <circle cx="12" cy="12" r="10"></circle>
            <polyline points="12,6 12,12 16,14"></polyline>
          </svg>
          <span>Ожидаемое время восстановления: в течение часа</span>
        </div>

        <div class="maintenance-info__item">
          <svg class="maintenance-info__icon" xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
            <path d="M4 4h16c1.1 0 2 .9 2 2v12c0 1.1-.9 2-2 2H4c-1.1 0-2-.9-2-2V6c0-1.1.9-2 2-2z"></path>
            <polyline points="22,6 12,13 2,6"></polyline>
          </svg>
          <span>По вопросам обращайтесь: support@tracknsave.com</span>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped lang="scss">
.maintenance-page {
  background-color: var(--vt-c-background);
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 0 20px;

  @media (max-width: 768px) {
    align-items: flex-start;
  }
}

.maintenance-content {
  background-color: var(--vt-c-dark-blue-gray);
  border-radius: 16px;
  padding: 48px;
  text-align: center;
  max-width: 600px;
  width: 100%;
  box-shadow: 0 4px 6px -1px rgba(0, 0, 0, 0.1);

  @media (max-width: 768px) {
    padding: 32px 20px;
    border-radius: 12px;
    max-width: 100%;
  }
}

.maintenance-icon {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  width: 96px;
  height: 96px;
  background-color: rgba(137, 225, 89, 0.2);
  border-radius: 50%;
  margin-bottom: 32px;
  color: var(--primary-green);

  @media (max-width: 768px) {
    width: 72px;
    height: 72px;
    margin-bottom: 24px;

    svg {
      width: 40px;
      height: 40px;
    }
  }
}

.maintenance-title {
  font-size: 32px;
  font-weight: 700;
  color: var(--vt-c-white);
  margin: 0 0 24px 0;

  @media (max-width: 768px) {
    font-size: 24px;
    margin: 0 0 20px 0;
  }
}

.maintenance-message {
  font-size: 18px;
  color: var(--vt-c-light-gray);
  margin: 0 0 16px 0;
  line-height: 1.6;

  @media (max-width: 768px) {
    font-size: 16px;
    margin: 0 0 12px 0;
  }
}

.maintenance-submessage {
  font-size: 16px;
  color: var(--vt-c-blue-gray);
  margin: 0 0 40px 0;
  line-height: 1.6;

  @media (max-width: 768px) {
    font-size: 14px;
    margin: 0 0 32px 0;
  }
}

.maintenance-info {
  border-top: 1px solid rgba(255, 255, 255, 0.1);
  padding-top: 32px;
  display: flex;
  flex-direction: column;
  gap: 16px;

  @media (max-width: 768px) {
    padding-top: 24px;
    gap: 12px;
  }

  &__item {
    display: flex;
    align-items: center;
    gap: 12px;
    color: var(--vt-c-blue-gray);
    font-size: 14px;
    text-align: left;

    @media (max-width: 768px) {
      font-size: 13px;
      gap: 8px;
      flex-direction: column;
      text-align: center;

      span {
        text-align: center;
      }
    }
  }

  &__icon {
    color: var(--primary-green);
    flex-shrink: 0;

    @media (max-width: 768px) {
      width: 16px;
      height: 16px;
    }
  }
}
</style>
