<template>
  <div>
    <nav class="navigation">
      <ul class="navigation__container">
        <li class="navigation__item"><router-link to="/">Главная</router-link></li>
        <li class="navigation__item"><router-link to="/receipt">Чеки</router-link></li>
        <li class="navigation__item"><router-link to="/profile">Помощь</router-link></li>
        <li class="navigation__item"><router-link to="/profile">Профиль</router-link></li>
        <li class="navigation__item"><router-link to="/profile">Выйти</router-link></li>
      </ul>
    </nav>
  </div>
</template>

<script lang="ts" setup>
</script>

<style lang="scss" scoped>
.navigation {
  box-sizing: border-box;

  &__container {
    display: flex;
    justify-content: space-between;
    width: 100%;
    list-style: none;
    gap: clamp(10px, 2vw, 40px);
    margin: 0;
    padding: 0;
  }

  &__item {
    position: relative;
    white-space: nowrap;
    a {
      padding: 10px;
      text-decoration: none;
      color: var(--vt-c-white);
      font-size: 18px;
      font-weight: 600;
      transition: 0.3s;

      @media (hover: hover) and (pointer: fine) {
        &:hover {
          color: var(--primary-green);
          transition: 0.3s;
        }
      }
    }
  }
}
</style>
