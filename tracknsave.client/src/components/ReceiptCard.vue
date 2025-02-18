<template>
  <div class="receipt">
    <article class="receipt__card">
      <header class="receipt__header">
        <div class="receipt__info">
          <div class="receipt__date-mobile">
            {{ formatDate(receipt.dateTime) }}
          </div>
          <div class="receipt__main-row">
            <div class="receipt__date-desktop">
              {{ formatDate(receipt.dateTime) }}
            </div>
            <div class="receipt__store">{{ receipt.retailPlace }}</div>
            <div class="receipt__total">
              {{ formatPrice(receipt.totalSum) }} ₽
            </div>
          </div>
        </div>
        <button
          class="receipt__toggle"
          :class="{ 'receipt__toggle--active': isExpanded }"
          type="button"
          @click="isExpanded = !isExpanded"
          :aria-expanded="isExpanded"
          aria-label="Развернуть детали чека"
        >
          <svg
            class="receipt__toggle-icon"
            viewBox="0 0 24 24"
            width="24"
            height="24"
          >
            <path d="M7 10l5 5 5-5H7z" fill="currentColor" />
          </svg>
        </button>
      </header>

      <div v-if="isExpanded" class="receipt__content">
        <ul class="receipt__list">
          <li
            v-for="item in receipt.items"
            :key="item.name"
            class="receipt__item"
          >
            <div class="receipt__item-header">
              <h3 class="receipt__item-title">{{ item.name }}</h3>
              <span class="receipt__item-sum"
                >{{ formatPrice(item.sum) }} ₽</span
              >
            </div>
            <div class="receipt__item-details">
              <span class="receipt__item-price"
                >{{ formatPrice(item.price) }} ₽ ×</span
              >
              <span class="receipt__item-quantity">{{ item.quantity }}</span>
            </div>
          </li>
        </ul>
      </div>
    </article>
  </div>
</template>

<script setup lang="ts">
  import { ref } from "vue";

  interface ReceiptItem {
    name: string;
    price: number;
    quantity: number;
    sum: number;
  }

  interface ReceiptData {
    user: string;
    totalSum: number;
    items: ReceiptItem[];
    dateTime: string;
    retailPlace: string;
  }

  defineProps<{
    receipt: ReceiptData;
  }>();

  const isExpanded = ref(false);

  const formatDate = (date: string): string => {
    return new Date(date).toLocaleDateString("ru-RU");
  };

  const formatPrice = (price: number): string => {
    return (price / 100).toFixed(2);
  };
</script>

<style lang="scss" scoped>
  .receipt {
    max-width: 1280px;
    width: 100%;
    margin: 0 auto;
    padding: 0 16px;

    &__card {
      background-color: var(--vt-c-dark-blue-gray);
      border-radius: 12px;
      box-shadow: 0 4px 6px -1px rgb(0, 0, 0, 0.1);
    }

    &__header {
      display: flex;
      align-items: center;
      gap: 16px;
      padding: 20px 24px;
    }

    &__info {
      flex: 1;
      min-width: 0;
    }

    &__main-row {
      display: flex;
      align-items: center;
      justify-content: space-between;
      gap: 16px;
    }

    &__store {
      color: var(--vt-c-white);
      font-size: 18px;
      font-weight: 600;
      white-space: nowrap;
      overflow: hidden;
      text-overflow: ellipsis;
    }

    &__date-desktop,
    &__date-mobile {
      color: var(--vt-c-white);
      font-size: 16px;
      font-weight: 500;
    }

    &__date-mobile {
      display: none;
      margin-bottom: 8px;
    }

    &__total {
      color: var(--vt-c-white);
      font-size: 18px;
      font-weight: 600;
      white-space: nowrap;
    }

    &__toggle {
      flex-shrink: 0;
      width: 32px;
      height: 32px;
      padding: 4px;
      border: none;
      border-radius: 6px;
      background-color: transparent;
      color: var(--vt-c-white);
      transition: all 0.2s ease;

      @media (hover: hover) and (pointer: fine) {
        &:hover {
          background-color: rgb(255, 255, 255, 0.1);
        }
      }

      &:focus-visible {
        outline: 2px solid var(--vt-c-white);
        outline-offset: 2px;
      }

      &--active {
        .receipt__toggle-icon {
          transform: rotate(180deg);
        }
      }
    }

    &__toggle-icon {
      transition: transform 0.3s ease;
    }

    &__content {
      border-top: 1px solid rgb(255, 255, 255, 0.1);
    }

    &__list {
      list-style: none;
      margin: 0;
      padding: 16px 24px;
    }

    &__item {
      padding: 12px 0;

      &:not(:last-child) {
        border-bottom: 1px solid rgb(255, 255, 255, 0.05);
      }
    }

    &__item-header {
      display: flex;
      justify-content: space-between;
      gap: 16px;
      margin-bottom: 4px;
    }

    &__item-title {
      margin: 0;
      color: var(--vt-c-light-gray);
      font-size: 15px;
      font-weight: 600;
      @media (max-width: 768px) {
        font-weight: 500;
      }
    }

    &__item-sum {
      color: var(--vt-c-white);
      font-size: 15px;
      font-weight: 500;
      white-space: nowrap;
      flex-shrink: 0;
    }

    &__item-details {
      display: flex;
      align-items: center;
      gap: 6px;
      color: var(--vt-c-blue-gray);
      font-size: 14px;
    }

    @media (max-width: 768px) {
      &__date-mobile {
        display: block;
      }

      &__date-desktop {
        display: none;
      }

      &__store,
      &__total {
        font-size: 16px;
      }

      &__header {
        padding: 16px 20px;
      }

      &__list {
        padding: 12px 20px;
      }

      &__item {
        padding: 10px 0;
      }
    }
  }
</style>
