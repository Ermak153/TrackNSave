<template>
  <div>
    <button
      class="burger"
      :class="{ 'burger--open': isOpen }"
      @click="toggleMenu"
    >
      <span></span>
      <span></span>
      <span></span>
    </button>

    <div class="menu" :class="{ 'menu--open': isOpen }">
      <button class="menu__close" @click="closeMenu">&times;</button>

      <ul class="menu__list">
        <li>
          <router-link to="/" @click="closeMenu">Главная</router-link>
        </li>
        <li>
          <router-link to="/receipt" @click="closeMenu">Чеки</router-link>
        </li>
        <li>
          <router-link to="/profile" @click="closeMenu">Помощь</router-link>
        </li>
        <li>
          <router-link to="/profile" @click="closeMenu">Профиль</router-link>
        </li>
        <li>
          <router-link to="/profile" @click="closeMenu">Выйти</router-link>
        </li>
      </ul>
    </div>
  </div>
</template>

<script setup lang="ts">
  import { ref } from "vue";

  const isOpen = ref(false);

  const toggleMenu = () => {
    isOpen.value = !isOpen.value;
  };

  const closeMenu = () => {
    isOpen.value = false;
  };
</script>

<style scoped lang="scss">
  .burger {
    width: 30px;
    height: 20px;
    display: flex;
    flex-direction: column;
    justify-content: space-between;
    background: none;
    border: none;
    z-index: 1100;

    &--open {
      display: none;
    }

    span {
      display: block;
      height: 2px;
      background: var(--vt-c-white);
      transition: all 0.3s ease;
    }
  }

  .menu {
    position: fixed;
    top: 0;
    right: 0;
    width: 100vw;
    height: 100vh;
    background: var(--color-background);
    display: flex;
    flex-direction: column;
    justify-content: center;
    align-items: center;
    z-index: 1000;
    overflow: hidden;
    box-shadow: -10px 0 30px rgba(0, 0, 0, 0.3);
    transform: translateX(100%);
    transition: transform 0.4s ease;

    &--open {
      transform: translateX(0);
    }

    &__close {
      position: absolute;
      top: 20px;
      left: 20px;
      font-size: 32px;
      background: none;
      border: none;
      color: var(--vt-c-white);
      cursor: pointer;
      transition: color 0.3s;

      @media (hover: hover) and (pointer: fine) {
        &:hover {
          color: var(--primary-green);
        }
      }
    }

    &__list {
      list-style: none;
      padding: 0;
      margin: 0;
      text-align: center;

      li {
        margin: 20px 0;

        a {
          text-decoration: none;
          color: var(--vt-c-white);
          font-size: 32px;
          font-weight: 700;
          transition: color 0.3s;

          @media (hover: hover) and (pointer: fine) {
            &:hover {
              color: var(--primary-green);
            }
          }
        }
      }
    }
  }
</style>
