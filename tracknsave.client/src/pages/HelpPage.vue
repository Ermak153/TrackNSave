<template>
  <div class="help">
    <div class="help__container">
      <div class="help__header">
        <h1 class="help__title">Помощь</h1>
        <p class="help__subtitle">Часто задаваемые вопросы и полезная информация</p>
      </div>

      <div class="help__sections">
        <div class="help__section">
          <h2 class="help__section-title">Основные вопросы</h2>
          <div class="help__faq">
            <div class="help__faq-item">
              <div class="help__faq-question">
                <h3 class="help__faq-title">Как добавить новый чек?</h3>
              </div>
              <div class="help__faq-answer">
                <p>Чтобы добавить новый чек, нажмите на кнопку "Добавить чек" на странице "Чеки". Затем вы можете выбрать удобный способ добавления чека.</p>
              </div>
            </div>

            <div class="help__faq-item">
              <div class="help__faq-question">
                <h3 class="help__faq-title">Как редактировать чек?</h3>
              </div>
              <div class="help__faq-answer">
                <p>Для редактирования чека найдите его в списке чеков и нажмите на кнопку "Изменить". Вы сможете изменить любые данные чека, включая товары и их количество. Изменять можно только неверифицированные чеки.</p>
              </div>
            </div>

            <div class="help__faq-item">
              <div class="help__faq-question">
                <h3 class="help__faq-title">Как удалить чек?</h3>
              </div>
              <div class="help__faq-answer">
                <p>Чтобы удалить чек, найдите его в списке чеков и нажмите на кнопку "Удалить". Подтвердите удаление в появившемся диалоговом окне.</p>
              </div>
            </div>
          </div>
        </div>

        <div class="help__section">
          <h2 class="help__section-title">Статистика и аналитика</h2>
          <div class="help__faq">
            <div class="help__faq-item">
              <div class="help__faq-question">
                <h3 class="help__faq-title">Как посмотреть статистику расходов?</h3>
              </div>
              <div class="help__faq-answer">
                <p>Общее количество чеков, общую сумму расходов и популярную категорию можно увидеть на странице профиля.</p>
              </div>
            </div>

            <div class="help__faq-item">
              <div class="help__faq-question">
                <h3 class="help__faq-title">Как посмотреть динамику цены на товар?</h3>
              </div>
              <div class="help__faq-answer">
                <p>Найдите необходимый товар на странице чеков и нажмите по нему. Отобразится график с подробной динамикой цены на выбранный товар. Если данных для отображения динамики цены недостаточно, будет отображено сообщение "История цен не найдена".</p>
              </div>
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
.help {
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
  }

  &__subtitle {
    font-size: 16px;
    color: var(--vt-c-light-gray);
    margin: 0;
  }

  &__search {
    margin-bottom: 32px;
  }

  &__search-wrapper {
    position: relative;
    display: flex;
    align-items: center;
  }

  &__search-icon {
    position: absolute;
    left: 16px;
    color: var(--vt-c-blue-gray);
    width: 20px;
    height: 20px;
  }

  &__search-input {
    width: 100%;
    padding: 12px 16px 12px 48px;
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

  &__faq {
    display: flex;
    flex-direction: column;
    gap: 16px;
  }

  &__faq-item {
    border: 1px solid var(--vt-c-blue-gray);
    border-radius: 8px;
    overflow: hidden;
  }

  &__faq-question {
    display: flex;
    align-items: center;
    justify-content: space-between;
    padding: 16px;
    background-color: var(--vt-c-light-background);
    cursor: pointer;
    transition: background-color 0.2s;

    &:hover {
      background-color: var(--vt-c-dark-blue-gray);
    }
  }

  &__faq-title {
    font-size: 16px;
    font-weight: 600;
    color: var(--vt-c-white);
    margin: 0;
  }

  &__faq-icon {
    color: var(--primary-green);
    width: 20px;
    height: 20px;
    transition: transform 0.2s;
  }

  &__faq-answer {
    padding: 16px;
    background-color: var(--vt-c-dark-blue-gray);
    color: var(--vt-c-light-gray);
    font-size: 14px;
    line-height: 1.5;
  }
}
</style>
