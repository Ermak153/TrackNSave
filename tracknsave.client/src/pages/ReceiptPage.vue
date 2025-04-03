<template>
  <div class="receipt__container">
    <div class="receipt__list">
      <SummaryCard />
      <h2 class="receipt__history">История</h2>
      <ReceiptCard
        v-for="receipt in state.receipts"
        :key="receipt.id"
        :receipt="{
          id: receipt.id,
          isVerified: receipt.isVerified,
          ...receipt.receiptData
        }"
      />

      <div class="receipt__is-empty" v-if="!state.receipts.length">
        <span>У вас пока нет добавленных чеков.</span>
        <span>Добавьте чеки, чтобы они появились здесь.</span>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
  import { onMounted } from "vue";
  import { useReceipts } from "@/composables/useReceipts";
  import { useAuth } from '@/composables/useAuth.ts';
  import SummaryCard from "@/components/SummaryCard.vue";
  import ReceiptCard from "@/components/ReceiptCard.vue";

  const { state, fetchReceipts } = useReceipts();
  const { isAuthenticated, checkAuthStatus } = useAuth();

  onMounted(async () => {
    await checkAuthStatus();
    if (isAuthenticated.value) {
      await fetchReceipts();
    }
  });
</script>

<style lang="scss" scoped>
  .receipt {
    &__container {
      width: 100%;
      display: flex;
      justify-content: center;
      box-sizing: border-box;
      padding: 0 20px;
    }

    &__list {
      width: 100%;
      max-width: 1280px;
      display: flex;
      flex-direction: column;
      gap: 15px;
      box-sizing: border-box;
    }

    &__history {
      color: var(--vt-c-white);
      margin-bottom: 0;
    }

    &__is-empty {
      display: flex;
      flex-direction: column;
      justify-content: center;
      align-items: center;
      margin-top: 30px;
      color: var(--vt-c-light-gray);
      text-align: center;
      font-size: 18px;
    }
  }

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
