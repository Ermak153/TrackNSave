<template>
  <div
    class="profile"
    v-if="isAuthenticated"
    @mouseenter="isMenuVisible = true"
    @mouseleave="isMenuVisible = false"
  >
    <router-link to="/profile">
      <ElAvatar
        class="profile__avatar"
        :size="50"
        src="https://cube.elemecdn.com/3/7c/3ea6beec64369c2642b92c6726f1epng.png"
      />
    </router-link>
    <Transition name="profile__menu-fade">
      <div class="profile__menu" v-show="isMenuVisible">
        <div class="profile__username-container">
          <span class="profile__username">
            {{ username }}
          </span>
        </div>
        <ul class="avatar__menu-list">
          <li><router-link to="/profile">Профиль</router-link></li>
          <li><router-link to="/receipt">Мои чеки</router-link></li>
          <li class="avatar__menu-list--exit">
            <router-link to="/login" @click="handleLogout">Выйти</router-link>
          </li>
        </ul>
      </div>
    </Transition>
  </div>
</template>

<script setup lang="ts">
  import { useAuth } from "@/composables/useAuth.ts";
  import { ElAvatar } from "element-plus";
  import { ref, onMounted } from "vue";
  const { isAuthenticated, logout, getUserInfo, username } = useAuth();

  let isMenuVisible = ref(false);

  const handleLogout = async () => {
    await logout();
    isMenuVisible = ref(false);
  };

  onMounted(async () => {
    await getUserInfo();
  });
</script>

<style lang="scss" scoped>
  .profile {
    position: relative;
    z-index: 2;
    align-items: center;

    &__avatar {
      position: relative;
      z-index: 3;
    }

    &__username-container {
      z-index: 3;
      width: 120px;
      height: 50px;
      margin: 10px;
      display: flex;
      align-items: center;
      justify-content: center;
    }

    &__username {
      color: var(--vt-c-white);
      font-weight: bold;
      white-space: nowrap;
      overflow: hidden;
      text-overflow: ellipsis;
      font-size: 18px;
    }

    &__menu {
      position: absolute;
      top: -10px;
      right: -10px;
      width: 200px;
      height: 200px;
      background: var(--vt-c-dark-blue-gray);
      box-shadow: 0 4px 15px rgba(0, 0, 0, 0.1);
      border-radius: 12px;
    }

    &__menu-fade-enter-active,
    &__menu-fade-leave-active {
      transition: all 0.3s ease;
      opacity: 1;
      transform: scale(1);
      pointer-events: auto;
    }

    &__menu-fade-enter-from,
    &__menu-fade-leave-to {
      opacity: 0;
      transform: scale(0);
      transform-origin: 80% 20%;
      pointer-events: none;
    }
  }

  .avatar {
    &__menu-list {
      width: 200px;
      height: 130px;
      margin: 0;
      padding: 0;
      list-style: none;
      border-top: 1px solid rgb(255, 255, 255, 0.1);

      &--exit {
        a {
          @media (hover: hover) and (pointer: fine) {
            &:hover {
              color: var(--vt-c-light-red) !important;
              transition: 0.2s;
            }
          }
        }
      }

      li {
        color: var(--vt-c-white);
        display: flex;
        justify-content: center;

        a {
          display: flex;
          justify-content: center;
          align-items: center;
          text-decoration: none;
          color: var(--vt-c-white);
          font-family: "Segoe UI";
          font-weight: 500;
          font-size: 18px;
          width: 100%;
          height: 40px;
          margin: 0;
          padding: 0;
          transition: 0.2s;

          @media (hover: hover) and (pointer: fine) {
            &:hover {
              color: var(--primary-green);
              transition: 0.2s;
            }
          }
        }
      }
    }
  }
</style>
