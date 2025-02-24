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
          <router-link to="/login" @click="handleLogoutAndClose">Выйти</router-link>
        </li>
      </ul>

      <div class="profile">
        <span class="profile__username">{{ username }}</span>
        <router-link to="/profile" class="profile__link" @click="closeMenu">
          <ElAvatar
            class="profile__avatar"
            :size="50"
            src="https://cube.elemecdn.com/3/7c/3ea6beec64369c2642b92c6726f1epng.png"
          />
        </router-link>
      </div>

    </div>
  </div>
</template>

<script setup lang="ts">
  import { useAuth } from "@/composables/useAuth.ts";
  import { ElAvatar } from "element-plus";
  import { ref, onMounted } from "vue";
  const {getUserInfo, username, logout } = useAuth();

  onMounted(async () => {
    await getUserInfo();
  });

  const isOpen = ref(false);

  const handleLogoutAndClose = async () => {
    await logout();
    closeMenu();
};

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
    justify-content: space-between;
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
    }

    &__list {
      list-style: none;
      padding: 0;
      margin: 0;
      margin-top: 20vh;
      text-align: center;
      display: flex;
      flex-direction: column;
      gap: 20px;

      li a {
        text-decoration: none;
        color: var(--vt-c-white);
        font-size: 32px;
        font-weight: 700;
        -webkit-tap-highlight-color: transparent;
      }
    }
  }

  .profile {
    display: flex;
    align-items: center;
    gap: 15px;
    color: var(--vt-c-white);
    padding: 20px;
    margin-bottom: 8vh;
    -webkit-tap-highlight-color: transparent;

    &__username {
      max-width: 150px;
      white-space: nowrap;
      overflow: hidden;
      text-overflow: ellipsis;
      font-size: 20px;
      font-weight: bold;
    }

    &__link {
      display: flex;
      align-items: center;
    }

    &__avatar {
      flex-shrink: 0;
    }
  }
</style>

