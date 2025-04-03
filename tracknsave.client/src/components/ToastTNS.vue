<template>
  <transition name="toast">
    <div v-if="show" class="toast" :class="type">
      {{ message }}
    </div>
  </transition>
</template>

<script setup lang="ts">
  import { ref } from "vue";

  const show = ref(false);
  const message = ref("");
  const type = ref("");

  const showToast = (msg: string, toastType: "success" | "error" | "info") => {
    message.value = msg;
    type.value = `toast--${toastType}`;
    show.value = true;

    setTimeout(() => {
      show.value = false;
    }, 3000);
  };

  defineExpose({
    showToast,
  });
</script>

<style lang="scss" scoped>
  .toast {
    position: fixed;
    bottom: 24px;
    left: 50%;
    transform: translateX(-50%);
    padding: 12px 20px;
    border-radius: 8px;
    color: var(--vt-c-white);
    font-size: 16px;
    font-weight: 600;
    z-index: 1000;
    box-shadow: 0 8px 16px rgba(0, 0, 0, 0.3), 0 -6px 12px rgba(0, 0, 0, 0.2),
      0 1px 3px rgba(0, 0, 0, 0.25);

    &--success {
      background: var(--primary-green);
      color: var(--vt-c-dark-blue-gray);
    }

    &--error {
      background: var(--vt-c-light-red);
    }

    &--info {
      background: var(--vt-c-dark-blue-gray);
    }
  }

  .toast-enter-active,
  .toast-leave-active {
    transition: all 0.3s ease;
  }

  .toast-enter-from,
  .toast-leave-to {
    opacity: 0;
    transform: translate(-50%, 20px);
  }
</style>
