<template>
  <header class="header">
    <div class="header__container">
      <IconHeader to="/" />
      <NavigationComponent class="header__navigation"/>
      <div class="header__mobile-navigation">
        <router-link
          v-if="!isAuthenticated"
          to="/login"
          class="header__button"
        >
          Войти
        </router-link>
        <ProfileMenu v-if="isAuthenticated" class="header__profile"/>
        <BurgerMenu v-if="isAuthenticated" class="header__burgermenu"/>
      </div>
    </div>
  </header>
</template>

<script setup lang="ts">
  import IconHeader from "@/components/IconHeader.vue";
  import NavigationComponent from "./NavigationComponent.vue";
  import BurgerMenu from "./BurgerMenu.vue";
  import ProfileMenu from "./ProfileMenu.vue";
  import { onMounted } from 'vue';
  import { useAuth } from '@/composables/useAuth.ts';
  const { isAuthenticated, checkAuthStatus } = useAuth();

  onMounted(async () => {
    await checkAuthStatus();
  });
</script>

<style lang="scss" scoped>
  .header {
    position: fixed;
    top: 0;
    left: 0;
    width: 100%;
    height: 128px;
    display: flex;
    align-items: center;
    backdrop-filter: blur(35px);
    z-index: 999;

    &__container {
      width: 100%;
      max-width: 1280px;
      margin: 0 auto;
      display: flex;
      align-items: center;
      justify-content: space-between;
      padding: 0 20px;
    }

    &__mobile-navigation {
      display: flex;
      align-items: center;
    }

    &__burgermenu {
      display: none;
    }

    &__profile {
      display: flex;
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

      &__avatar {
        display: none;
      }

      &__button {
        font-size: 14px;
        padding: 10px 20px;
      }

      &__navigation {
        display: none;
      }

      &__burgermenu {
        display: flex;
        margin-left: 20px;
      }

      &__profile {
        display: none;
      }
    }
  }
</style>
