<template>
  <DefaultLayout>

  </DefaultLayout>
</template>

<script lang="ts" setup>
  import DefaultLayout from '@/layouts/DefaultLayout.vue'
  import { onMounted, onUnmounted, ref } from 'vue';
  import { useAuth } from '@/composables/useAuth';

  const auth = useAuth();

  const userActivityTimeout = ref<number | null>(null);
  const tokenRefreshInterval = ref<number | null>(null);

  const handleUserActivity = () => {
    if (userActivityTimeout.value) {
      clearTimeout(userActivityTimeout.value);
    }

    userActivityTimeout.value = window.setTimeout(async () => {
      if (auth.isAuthenticated.value) {
        await auth.refreshToken();
      }
    }, 30000);
  };

  onMounted(() => {
    tokenRefreshInterval.value = auth.setupTokenRefreshTimer();

    window.addEventListener('click', handleUserActivity);
    window.addEventListener('keydown', handleUserActivity);

    auth.checkAuthStatus(true);
  });

  onUnmounted(() => {
    if (userActivityTimeout.value) {
      clearTimeout(userActivityTimeout.value);
    }

    if (tokenRefreshInterval.value) {
      clearInterval(tokenRefreshInterval.value);
    }

    window.removeEventListener('click', handleUserActivity);
    window.removeEventListener('keydown', handleUserActivity);
  });
</script>

<style lang="scss" scoped>

</style>
