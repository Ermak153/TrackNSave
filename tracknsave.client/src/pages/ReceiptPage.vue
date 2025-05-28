<template>
  <div class="receipt__container">
    <div class="receipt__list">
      <DashBoard
      :totalReceipts="totalReceipts"
      :totalAmount="totalAmount"
      :sort="sortOption"
      @update:sort="setSortOption"
      @update:dateRange="updateDateRange" />
      <h2 class="receipt__history">История</h2>
      <TransitionGroup
        name="receipt-list"
        tag="div"
        class="receipt__cards-container">
        <ReceiptCard
          v-for="receipt in filteredReceipts"
          :key="receipt.id"
          :receipt="{
            id: receipt.id,
            isVerified: receipt.isVerified,
            ...receipt.receiptData
          }"
        />
      </TransitionGroup>
      <Transition name="fade">
        <div class="receipt__is-empty" v-if="!state.receipts.length">
          <span>У вас пока нет добавленных чеков.</span>
          <span>Добавьте чеки, чтобы они появились здесь.</span>
        </div>
      </Transition>
    </div>
  </div>
</template>

<script setup lang="ts">
  import { onMounted, ref } from "vue";
  import { useReceipts } from "@/composables/useReceipts";
  import { useAuth } from '@/composables/useAuth.ts';
  import DashBoard from "@/components/DashBoard.vue";
  import ReceiptCard from "@/components/ReceiptCard.vue";

  const { state, sortOption, filteredReceipts, totalAmount, totalReceipts, setDateRange, fetchReceipts, setSortOption } = useReceipts();
  const { isAuthenticated, checkAuthStatus } = useAuth();

  const dateRange = ref<[string | null, string | null]>([null, null]);

  const updateDateRange = (value: [string | null, string | null]) => {
    dateRange.value = value;
    setDateRange(value);
  };

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
    }

    &__cards-container {
      width: 100%;
      max-width: 1280px;
      display: flex;
      flex-direction: column;
      gap: 15px;
      box-sizing: border-box;
    }

    &__history {
      color: var(--vt-c-white);
      margin-bottom: 10px;
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

  .receipt-list-move,
  .receipt-list-enter-active,
  .receipt-list-leave-active {
    transition: all 0.5s ease;
  }

  .receipt-list-enter-from,
  .receipt-list-leave-to {
    opacity: 0;
    transform: translateY(30px);
  }

  .receipt-list-leave-active {
    position: absolute;
  }

  .fade-enter-active,
  .fade-leave-active {
    transition: opacity 0.5s ease;
  }

  .fade-enter-from,
  .fade-leave-to {
    opacity: 0;
  }
</style>
