<template>
  <div class="receipt__container">
    <div class="receipt__list">
      <SummaryCard />
      <h2 class="receipt__history">История</h2>
      <ReceiptCard
        v-for="receipt in state.receipts"
        :key="receipt.id"
        :receipt="receipt.receiptData"
      />
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
  }
</style>
