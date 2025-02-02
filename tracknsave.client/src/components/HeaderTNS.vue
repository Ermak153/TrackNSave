<template>
  <header class="header">
    <div class="header__container">
      <IconHeader to="/"></IconHeader>
      <router-link to="/login" class="header__button">Войти</router-link>
    </div>
  </header>
</template>

<script setup lang="ts">
  import IconHeader from "@/components/IconHeader.vue";
</script>

<style lang="scss" scoped>
  .header {
    position: fixed;
    top: 0;
    left: 0;
    width: 100%;
    height: 180px;
    display: flex;
    align-items: center;
    background: transparent;
    backdrop-filter: blur(35px);
    z-index: 1000;

    &__container {
      width: 100%;
      max-width: 1280px;
      margin: 0 auto;
      display: flex;
      align-items: center;
      justify-content: space-between;
      padding: 0 20px;
    }

    &__logo svg {
      height: 100px;
    }

    &__button {
      background-color: transparent;
      color: var(--primary-green);
      font-size: 16px;
      font-weight: 700;
      border: 2px solid var(--primary-green);
      border-radius: 8px;
      padding: 12px 24px;
      transition: 0.3s;
      text-decoration: none;

      @media (hover: hover) and (pointer: fine) {
        &:hover {
          background-color: var(--primary-green);
          color: var(--color-background);
          border-color: transparent;
        }
      }
    }
  }
  @media (max-width: 768px) {
    .header {
      height: 100px;

      &__button {
        font-size: 14px;
        padding: 10px 20px;
      }
    }
  }
</style>
