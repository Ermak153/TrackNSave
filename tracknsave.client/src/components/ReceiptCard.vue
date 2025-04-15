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
        <div class="receipt__actions">
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

          <div
            class="receipt__menu-wrapper"
            ref="menuWrapper"
            @mouseenter="handleMouseEnter"
            @mouseleave="handleMouseLeave"
          >
            <button
              class="receipt__menu-btn"
              @click="toggleMenu"
              aria-label="Дополнительные действия"
            >
              <svg width="24" height="24" viewBox="0 0 24 24">
                <circle cx="12" cy="6" r="2" fill="currentColor" />
                <circle cx="12" cy="12" r="2" fill="currentColor" />
                <circle cx="12" cy="18" r="2" fill="currentColor" />
              </svg>
            </button>

            <ul v-if="isMenuOpen" class="receipt__menu">
              <li
                v-if="props.receipt.isVerified"
                @click="doAction('Сохранить')"
              >
                Сохранить
              </li>
              <li
                v-if="!props.receipt.isVerified"
                @click="doAction('Изменить')"
              >
                Изменить
              </li>
              <li @click="doAction('Удалить')">Удалить</li>
            </ul>
          </div>
        </div>
      </header>

      <div v-if="isExpanded" class="receipt__content">
        <ul class="receipt__list">
          <li
            v-for="item in receipt.items"
            :key="item.name"
            class="receipt__item"
            @click="openPriceChart(item.name)"
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
            <span>{{ item.category }}</span>
          </li>
        </ul>
      </div>

      <transition name="fade">
        <div v-if="showDeleteModal" class="delete-modal-overlay">
          <div class="delete-modal" @click.stop>
            <div class="delete-modal__content">
              <h3 class="delete-modal__title">Подтверждение удаления</h3>
              <p class="delete-modal__text">
                Вы уверены, что хотите удалить этот чек?
              </p>

              <div class="delete-modal__actions">
                <button
                  class="delete-modal__button delete-modal__button--cancel"
                  @click="cancelDelete"
                >
                  Отмена
                </button>
                <button
                  class="delete-modal__button delete-modal__button--confirm"
                  @click="confirmDelete"
                >
                  Удалить
                </button>
              </div>
            </div>
          </div>
        </div>
      </transition>
    </article>
    <EditReceipt
      v-model="isModalOpen"
      :receipt-id="receipt.id"
      :receipt="{
        RetailPlace: receipt.retailPlace,
        DateTime: receipt.dateTime,
        TotalSum: receipt.totalSum,
        Items: receipt.items.map((item) => ({
          Sum: item.sum,
          Name: item.name,
          Price: item.price,
          Quantity: item.quantity,
        })),
      }"
    ></EditReceipt>

    <PriceHistory
      v-if="selectedProduct"
      v-model="chartVisible"
      :productName="selectedProduct.productName"
      :priceHistory="selectedProduct.priceHistory"
    />
  </div>
</template>

<script setup lang="ts">
  import { ref, onMounted, onUnmounted } from "vue";
  import { useReceipts } from "@/composables/useReceipts";
  import { useToast } from "@/composables/useToast";
  import { useErrorHandler } from "@/composables/useErrorHandler";
  import api from "@/api/axios";
  import EditReceipt from "@/components/EditReceipt.vue";
  import PriceHistory from "@/components/PriceHistory.vue";

  const { deleteReceipt } = useReceipts();
  const toast = useToast();
  const { errorMessage, handleApiError } = useErrorHandler();

  const menuWrapper = ref<HTMLElement | null>(null);
  const closeTimeout = ref<number | null>(null);
  const isMobile = ref(false);
  const isMenuOpen = ref(false);
  const isModalOpen = ref(false);
  const isExpanded = ref(false);
  const showDeleteModal = ref(false);
  const isLoadingPdf = ref(false);

  const props = defineProps<{
    receipt: ReceiptData;
  }>();

  interface ReceiptItem {
    name: string;
    price: number;
    quantity: number;
    sum: number;
    category: string;
  }

  interface ReceiptData {
    id: number;
    user: string;
    totalSum: number;
    items: ReceiptItem[];
    dateTime: string;
    retailPlace: string;
    isVerified: boolean;
  }

  interface SelectedProduct {
    productName: string;
    priceHistory: {
      price: number;
      dateTime: string;
      retailPlace: string;
    }[];
  }

  const checkMobile = () => {
    isMobile.value = window.matchMedia("(hover: none)").matches;
  };

  const componentId = ref(
    Date.now().toString() + Math.random().toString(36).substr(2, 9)
  );

  const formatDate = (date: string): string => {
    return new Date(date).toLocaleDateString("ru-RU");
  };

  const formatPrice = (price: number): string => {
    return (price / 100).toFixed(2);
  };

  const toggleMenu = () => {
    if (isMobile.value) {
      if (isMenuOpen.value) {
        isMenuOpen.value = false;
        return;
      }

      window.dispatchEvent(
        new CustomEvent("menu-open", {
          detail: { id: componentId.value },
        })
      );

      isMenuOpen.value = true;
    }
  };

  const handleMenuOpen = (event: CustomEvent) => {
    if (event.detail.id !== componentId.value && isMenuOpen.value) {
      isMenuOpen.value = false;
    }
  };

  const handleClickOutside = (event: MouseEvent) => {
    if (
      (isMenuOpen.value &&
        menuWrapper.value &&
        !menuWrapper.value.contains(event.target as Node)) ||
      (showDeleteModal.value &&
        event.target &&
        (event.target as HTMLElement).classList.contains("delete-modal-overlay"))
    ) {
      isMenuOpen.value = false;
      if (showDeleteModal.value) {
        showDeleteModal.value = false;
        toast.show("Удаление отменено", "info");
      }
    }
  };

  const handleMouseEnter = () => {
    if (!isMobile.value) {
      if (closeTimeout.value) {
        clearTimeout(closeTimeout.value);
        closeTimeout.value = null;
      }
      isMenuOpen.value = true;
    }
  };

  const handleMouseLeave = () => {
    if (!isMobile.value) {
      closeTimeout.value = setTimeout(() => {
        isMenuOpen.value = false;
      }, 150);
    }
  };

  const doAction = async (action: string) => {
    switch (action) {
      case "Удалить":
        isMenuOpen.value = false;
        showDeleteModal.value = true;
        break;
      case "Изменить":
        isMenuOpen.value = false;
        isModalOpen.value = true;
        break;
      case "Сохранить":
        isMenuOpen.value = false;
        handleDownloadPdf();
        break;
    }
  };

  const cancelDelete = () => {
    showDeleteModal.value = false;
    toast.show("Удаление отменено", "info");
  };

  const confirmDelete = async () => {
    try {
      showDeleteModal.value = false;
      await deleteReceipt(props.receipt.id);
      toast.show("Чек успешно удален", "success");
    } catch (error) {
      handleApiError(error);
      toast.show(errorMessage.value ?? "Ошибка при удалении чека", "error");
    }
  };

  const handleDownloadPdf = async () => {
    isLoadingPdf.value = true;
    try {
      const response = await api.post(
        "/receipt/get-pdf",
        {
          ReceiptId: props.receipt.id,
        },
        {
          responseType: "blob",
        }
      );

      const blob = new Blob([response.data], { type: "application/pdf" });

      const url = window.URL.createObjectURL(blob);

      const fileName = `receipt_${props.receipt.id}.pdf`;
      const link = document.createElement("a");
      link.style.display = "none";
      link.href = url;
      link.download = fileName;
      document.body.appendChild(link);
      link.click();

      setTimeout(() => {
        document.body.removeChild(link);
        window.URL.revokeObjectURL(url);
      }, 100);

      toast.show("Чек успешно сохранен", "success");
    } catch (error) {
      handleApiError(error);
      toast.show(errorMessage.value ?? "Ошибка при сохранении чека", "error");
    } finally {
      isLoadingPdf.value = false;
    }
  };

  const chartVisible = ref(false);
  const selectedProduct = ref<SelectedProduct | null>(null);

  const openPriceChart = async (productName: string) => {
    try {
      const response = await api.post("/receipt/product-history", {
        productName: productName
      });

      const product = response.data?.productHistory?.[0];

      if (product && product.priceHistory.length > 1) {
        selectedProduct.value = product;
        chartVisible.value = true;
      } else {
        toast.show("История цен не найдена", "info");
      }
    } catch (error) {
      handleApiError(error);
      toast.show(errorMessage.value ?? "Ошибка при получении истории цен", "error");
    }
  };

  onMounted(() => {
    checkMobile();
    window.addEventListener("resize", checkMobile);
    document.addEventListener("click", handleClickOutside);
    window.addEventListener("menu-open", handleMenuOpen as EventListener);
  });

  onUnmounted(() => {
    if (closeTimeout.value) clearTimeout(closeTimeout.value);
    window.removeEventListener("resize", checkMobile);
    document.removeEventListener("click", handleClickOutside);
    window.removeEventListener("menu-open", handleMenuOpen as EventListener);
  });
</script>

<style lang="scss" scoped>
  .receipt {
    max-width: 1280px;
    width: 100%;
    box-sizing: border-box;
    margin: 0;
    padding: 0;

    &__actions {
      display: flex;
      align-items: center;
      justify-content: center;
    }

    &__menu {
      position: absolute;
      top: 100%;
      right: -1%;
      background: var(--vt-c-dark-blue-gray);
      border-radius: 6px;
      box-shadow: 0 8px 16px rgba(0, 0, 0, 0.3), 0 -6px 12px rgba(0, 0, 0, 0.2),
        0 1px 3px rgba(0, 0, 0, 0.25);
      list-style: none;
      margin: 8px 0;
      padding: 0;
      min-width: 120px;
      z-index: 10;
      box-sizing: border-box;

      li {
        display: block;
        box-sizing: border-box;
        cursor: pointer;
        color: var(--vt-c-white);
        font-size: 16px;
        transition: 0.2s;
        padding: 12px 18px;
        border-radius: 6px;

        @media (max-width: 768px) {
          font-size: 20px;
        }

        @media (hover: hover) and (pointer: fine) {
          &:hover {
            background: rgba(255, 255, 255, 0.1);
            transition: 0.1s;
          }
        }
      }
    }

    &__menu-wrapper {
      position: relative;
      display: flex;
      align-items: center;
    }

    &__menu-btn {
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
    }

    &__card {
      width: 100%;
      background-color: var(--vt-c-dark-blue-gray);
      border-radius: 12px;
      box-shadow: 0 4px 6px -1px rgba(0, 0, 0, 0.1);
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

  .delete-modal {
    background: var(--vt-c-dark-blue-gray);
    border-radius: 12px;
    max-width: 400px;
    width: 100%;
    box-shadow: 0 16px 32px rgba(0, 0, 0, 0.25);

    &-overlay {
      position: fixed;
      top: 0;
      left: 0;
      right: 0;
      bottom: 0;
      background: rgba(0, 0, 0, 0.65);
      display: flex;
      align-items: center;
      justify-content: center;
      z-index: 100;
      padding: 16px;
    }

    &__content {
      padding: 24px;
    }

    &__title {
      color: var(--vt-c-white);
      font-size: 20px;
      font-weight: 600;
      margin: 0 0 16px;
    }

    &__text {
      color: var(--vt-c-light-gray);
      font-size: 16px;
      margin: 0 0 24px;
      line-height: 1.5;
    }

    &__actions {
      display: flex;
      gap: 12px;
      justify-content: flex-end;
    }

    &__button {
      padding: 10px 16px;
      border-radius: 6px;
      font-size: 16px;
      font-weight: 600;
      border: none;
      cursor: pointer;
      transition: all 0.2s ease;

      &--cancel {
        background: transparent;
        color: var(--vt-c-white);
        border: 2px solid var(--vt-c-white);

        @media (hover: hover) and (pointer: fine) {
          &:hover {
            background: var(--vt-c-white);
            color: var(--vt-c-dark-blue-gray);
            transition: 0.2s;
          }
        }
      }

      &--confirm {
        background: var(--vt-c-light-red);
        color: var(--vt-c-white);
        border: 2px solid var(--vt-c-light-red);

        @media (hover: hover) and (pointer: fine) {
          &:hover {
            background: transparent;
            color: var(--vt-c-light-red);
            transition: 0.2s;
          }
        }
      }
    }

    @media (max-width: 768px) {
      &__content {
        padding: 20px;
      }

      &__button {
        padding: 12px 16px;
        flex: 1;
      }
    }
  }

  .fade-enter-active,
  .fade-leave-active {
    transition: opacity 0.2s ease;
  }

  .fade-enter-from,
  .fade-leave-to {
    opacity: 0;
  }
</style>
