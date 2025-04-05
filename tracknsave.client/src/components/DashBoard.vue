<template>
  <div class="dashboard">
    <div class="dashboard__info">
      <div class="info-card">
        <div class="info-card__icon-wrapper">
          <svg class="info-card__icon" xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
            <path d="M3 3v18h18"></path>
            <path d="M18 17v4"></path>
            <path d="M13 13v8"></path>
            <path d="M8 9v12"></path>
          </svg>
        </div>
        <div class="info-card__details">
          <h3 class="info-card__value">{{ totalReceipts }}</h3>
          <p class="info-card__label">Общее кол-во чеков</p>
        </div>
      </div>

      <div class="info-card">
        <div class="info-card__icon-wrapper">
          <svg class="info-card__icon" xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
            <circle cx="12" cy="12" r="10"></circle>
            <path d="M16 8h-6a2 2 0 1 0 0 4h4a2 2 0 1 1 0 4H8"></path>
            <path d="M12 18V6"></path>
          </svg>
        </div>
        <div class="info-card__details">
          <h3 class="info-card__value">{{ totalAmount }} ₽</h3>
          <p class="info-card__label">Общая сумма</p>
        </div>
      </div>
    </div>

    <div class="dashboard__actions">
      <div class="actions-filters">
        <div class="actions-filters__group">
          <label class="actions-filters__label">Сортировка</label>
          <select class="actions-filters__select" :value="sort" @change="handleSortChange">
            <option v-for="option in sortOptions" :key="option" :value="option">{{ option }}</option>
          </select>
        </div>

        <div class="actions-filters__group">
          <label class="actions-filters__label">Период</label>
          <DateRange v-model="dateRange" @update:modelValue="(value) => emit('update:dateRange', value)" class="actions-filters__date-range"/>
        </div>
      </div>

      <button class="dashboard__button" @click="showModal = true">Добавить чек</button>
      <ReceiptModal v-model="showModal"> </ReceiptModal>
    </div>
  </div>
</template>

<script lang="ts" setup>
  import { ref, defineEmits } from 'vue'
  import DateRange from './DateRange.vue';
  import ReceiptModal from "./ReceiptModal.vue";

  const dateRange = ref<[string | null, string | null]>([null, null]);
  const showModal = ref(false);

  const sortOptions = [
  'Недавно добавленные',
  'Сначала новые',
  'Сначала старые',
  'По сумме (возр.)',
  'По сумме (убыв.)'
];

  const handleSortChange = (event: Event) => {
    const target = event.target as HTMLSelectElement;
    emit('update:sort', target.value);
  };

  defineProps<{
    totalReceipts: number;
    totalAmount: number;
    sort: string;
  }>();

  const emit = defineEmits<{
    (e: 'update:sort', sort: string): void;
    (e: 'update:dateRange', value: [string | null, string | null]): void;
  }>();
</script>

<style lang="scss" scoped>

  .dashboard {
    width: 100%;
    max-width: 1280px;
    margin: 0;

    &__info {
      display: grid;
      grid-template-columns: repeat(2, 1fr);
      gap: 20px;
      margin-bottom: 24px;

      @media (max-width: 768px) {
        grid-template-columns: 1fr;
      }
    }

    &__actions {
      display: flex;
      justify-content: space-between;
      align-items: center;
      background-color: var(--vt-c-dark-blue-gray);
      box-shadow: 0 4px 6px -1px rgba(0, 0, 0, 0.1);
      border-radius: 16px;
      padding: 20px;

      @media (max-width: 768px) {
        flex-direction: column;
        align-items: stretch;
      }
    }

    &__button {
      background: var(--primary-green);
      color: var(--vt-c-dark-blue-gray);
      border: 2px solid var(--primary-green);
      padding: 22px 28px;
      border-radius: 8px;
      font-size: 18px;
      font-weight: bold;
      cursor: pointer;
      transition: 0.2s;

      @media (max-width: 768px) {
        padding: 14px 28px;
      }

      @media (hover: hover) and (pointer: fine) {
        &:hover {
          background: rgba(137, 225, 89, 0.2);
          color: var(--vt-c-white);
          border: 2px solid var(--primary-green);
          transition: 0.2s;
        }
      }
    }
  }

  .info-card {
    display: flex;
    align-items: center;
    background-color: var(--vt-c-dark-blue-gray);
    box-shadow: 0 4px 6px -1px rgba(0, 0, 0, 0.1);
    border-radius: 16px;
    padding: 20px;

    &__icon-wrapper {
      display: flex;
      align-items: center;
      justify-content: center;
      width: 48px;
      height: 48px;
      background-color: rgba(137, 225, 89, 0.2);
      border-radius: 12px;
      margin-right: 16px;
    }

    &__icon {
      color: var(--primary-green);
    }

    &__details {
      flex: 1;
    }

    &__value {
      font-size: 24px;
      font-weight: 700;
      color: var(--vt-c-white);
      margin: 0 0 4px 0;
    }

    &__label {
      font-size: 16px;
      color: var(--vt-c-light-gray);
      margin: 0;
    }
  }

  .actions-filters {
    display: flex;
    gap: 40px;
    flex-wrap: wrap;
    justify-content: space-between;
    box-sizing: border-box;

    @media (max-width: 768px) {
      flex-direction: column;
      margin-bottom: 16px;
      width: 100%;
      gap: 20px;
    }

    &__group {
      display: flex;
      flex-direction: column;
      gap: 8px;
    }

    &__label {
      font-size: 16px;
      color: var(--vt-c-light-gray);
      font-weight: 500;
    }

    &__select {
      background-color: var(--vt-c-light-background);
      border: 1px solid var(--vt-c-blue-gray);
      border-radius: 8px;
      padding: 13px 16px;
      color: var(--vt-c-white);
      font-size: 16px;
      outline: none;
      min-width: 180px;
      transition: 0.2s;
      cursor: pointer;
      position: relative;

      @media (max-width: 768px) {
        min-width: 140px;
      }

      @media (hover: hover) and (pointer: fine) {
        &:hover {
          border: 1px solid var(--primary-green);
          transition: 0.2s;
        }
      }
    }

    &__date-range {
      min-width: 300px;

      @media (max-width: 768px) {
        min-width: 140px;
      }
    }
  }
</style>
