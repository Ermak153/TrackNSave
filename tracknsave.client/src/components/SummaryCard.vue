<template>
  <div class="stats">
    <div class="stats__box">
      <div class="stats__item">
        <span class="stats__label">Общее кол-во чеков</span>
        <span class="stats__value">{{ totalReceipts }}</span>
      </div>
      <div class="stats__item">
        <span class="stats__label">Общая сумма</span>
        <span class="stats__value">{{ totalAmount }} ₽</span>
      </div>
    </div>
    <button class="stats__button" @click="showModal = true">Добавить чек</button>
    <ReceiptModal v-model="showModal"> </ReceiptModal>

  </div>
</template>

<script lang="ts" setup>
  import ReceiptModal from "./ReceiptModal.vue";
  import { ref } from "vue";

  const showModal = ref(false);

  const totalReceipts = ref(12);
  const totalAmount = ref(4320.5);
</script>

<style lang="scss" scoped>
.stats {
  display: flex;
  justify-content: space-between;
  align-items: center;
  background: var(--vt-c-dark-blue-gray);
  padding: 20px;
  border-radius: 12px;
  box-shadow: 0 4px 8px rgba(0, 0, 0, 0.2);
  flex-wrap: wrap;

  @media (max-width: 768px) {
    flex-direction: column;
    align-items: center;
    text-align: center;
  }

  &__box {
    display: flex;
    gap: 80px;
    margin-right: 80px;

    @media (max-width: 768px) {
      flex-direction: column;
      gap: 20px;
    }
  }

  &__item {
    display: flex;
    flex-direction: column;
    text-align: center;
    color: var(--vt-c-white);
  }

  &__label {
    font-size: 14px;
    font-weight: 500;
    color: var(--vt-c-blue-gray);
  }

  &__value {
    font-size: 20px;
    font-weight: bold;
  }

  &__controls {
    display: flex;
    gap: 15px;
    align-items: center;
    flex-wrap: wrap;

    @media (max-width: 768px) {
      flex-direction: column;
      width: 100%;
    }
  }

  &__search,
  &__filter,
  &__month {
    padding: 12px;
    border-radius: 8px;
    border: none;
    font-size: 16px;
    background: var(--vt-c-white);
    width: 200px;
    text-align: center;

    @media (max-width: 768px) {
      width: 100%;
    }
  }

  &__button {
    background: var(--primary-green);
    color: var(--vt-c-dark-blue-gray);
    border: 2px solid var(--primary-green);
    padding: 14px 28px;
    border-radius: 8px;
    font-size: 18px;
    font-weight: bold;
    cursor: pointer;
    transition: 0.2s;
    margin-top: 10px;

    &:hover {
      background: rgba(137, 225, 89, 0.2);
      color: var(--vt-c-white);
      border: 2px solid var(--primary-green);
      transition: 0.2s;
    }
  }
}
</style>
