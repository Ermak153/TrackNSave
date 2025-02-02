<template>
  <div>
    <AuthComponent class="auth-container"></AuthComponent>
  </div>
</template>

<script setup lang="ts">
  import AuthComponent from '@/components/AuthComponent.vue';
</script>

<style lang="scss" scoped>
  .auth-container {
    display: flex;
    justify-content: center;
    align-items: center;

    @media (max-width: 768px) {
      margin-top: 30px;
    }
  }
</style>
