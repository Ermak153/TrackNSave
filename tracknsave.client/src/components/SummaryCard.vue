<template>
  <div class="summary">
    <div class="summary__container">
      <div class="summary__card">
        <div class="summary__card-block">
          <h2>Общее кол-во чеков</h2>
          <span>{{ totalReceipts }}</span>
        </div>
        <div class="summary__card-block">
          <h2>Общая сумма</h2>
          <span>{{ totalAmount }}</span>
        </div>
      </div>
      <div class="summary__card">
        <div class="summary__card-block">
          <button class="summary__add-button" @click="showModal = true">
            Добавить чек
          </button>
        </div>
      </div>
    </div>
  </div>
  <ReceiptModal v-model="showModal"> </ReceiptModal>
</template>

<script setup lang="ts">
  import ReceiptModal from "./ReceiptModal.vue";
  import { ref } from "vue";

  const showModal = ref(false);

  const totalReceipts = ref(12);
  const totalAmount = ref(4320.5);
</script>

<style lang="scss" scoped>
  .summary {
    display: flex;
    justify-content: center;

    &__container {
      max-width: 1280px;
      width: 100%;
      display: flex;
      justify-content: space-between;
    }

    &__card {
      display: flex;
      background-color: var(--vt-c-dark-blue-gray);
      border-radius: 12px;
      padding: 10px;
      gap: 30px;
    }

    &__add-button {
      font-size: 20px;
      background: var(--primary-green);
      color: var(--vt-c-white);
      border: none;
      padding: 10px 15px;
      border-radius: 5px;
      border: 2px solid transparent;
      width: 100%;
      margin-top: 10px;
      box-sizing: border-box;
      transition: 0.2s;
      @media (hover: hover) and (pointer: fine) {
        &:hover {
          background: rgba(137, 225, 89, 0.2);
          border: 2px solid var(--primary-green);
          transition: 0.2s;
        }
      }
    }
  }
</style>
