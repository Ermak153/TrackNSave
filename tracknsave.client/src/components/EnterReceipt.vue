<template>
  <div class="enter-receipt">
    <div v-if="scanSuccess" class="enter-receipt__success-container">
      <div class="enter-receipt__success-screen">
        <div class="enter-receipt__success-icon">
          <svg
            width="80"
            height="80"
            viewBox="0 0 80 80"
            fill="none"
            xmlns="http://www.w3.org/2000/svg"
          >
            <circle
              cx="40"
              cy="40"
              r="38"
              stroke="var(--primary-green)"
              stroke-width="4"
            />
            <path
              d="M25 40L35 50L55 30"
              stroke="var(--primary-green)"
              stroke-width="4"
              stroke-linecap="round"
              stroke-linejoin="round"
            />
          </svg>
        </div>
        <p class="enter-receipt__success-message">Чек успешно добавлен</p>
      </div>

      <div class="enter-receipt__success-buttons">
        <button
          class="enter-receipt__button enter-receipt__button--cancel"
          @click="$emit('close')"
        >
          Закрыть
        </button>
        <button
          class="enter-receipt__button enter-receipt__button--submit"
          @click="resetForm"
        >
          Добавить ещё
        </button>
      </div>
    </div>

    <div class="enter-receipt__container" v-if="!scanSuccess">
      <span class="enter-receipt__error-message" v-if="errorMessage">{{
        errorMessage
      }}</span>
      <span class="enter-receipt__totalsum"
        >Итоговая сумма: {{ receipt.TotalSum }} ₽</span
      >

      <div class="enter-receipt__line"></div>

      <div class="enter-receipt__items-list">
        <div class="enter-receipt__header">
          <div>
            <label for="name" class="enter-receipt__label"
              >Название магазина</label
            >
            <input
              id="name"
              v-model="receipt.RetailPlace"
              placeholder="Название магазина"
              class="enter-receipt__input"
              @blur="validateField('RetailPlace')"
            />
            <span v-if="errors.RetailPlace" class="enter-receipt__error">{{
              errors.RetailPlace
            }}</span>
          </div>
          <div>
            <label for="datetime" class="enter-receipt__label"
              >Время покупки</label
            >
            <input
              v-model="receipt.DateTime"
              id="datetime"
              type="datetime-local"
              class="enter-receipt__input"
              @blur="validateField('DateTime')"
            />
            <span v-if="errors.DateTime" class="enter-receipt__error">{{
              errors.DateTime
            }}</span>
          </div>
        </div>

        <div class="enter-receipt__line"></div>

        <div
          v-for="(item, index) in receipt.Items"
          :key="index"
          class="enter-receipt__item"
        >
          <button
            @click="removeItem(index)"
            class="enter-receipt__remove-button"
          >
            &times;
          </button>

          <label for="item_name" class="enter-receipt__label"
            >Название товара</label
          >
          <input
            id="item_name"
            v-model="item.Name"
            placeholder="Название товара"
            class="enter-receipt__input"
            @input="handleInput(index)"
            @blur="validateField('Item', index, 'Name')"
          />
          <span
            v-if="errors[`Items.${index}.Name`]"
            class="enter-receipt__error"
          >
            {{ errors[`Items.${index}.Name`] }}
          </span>

          <div class="enter-receipt__inputs-group">
            <div>
              <label for="price" class="enter-receipt__label">Цена</label>
              <input
                id="price"
                v-model.number="item.Price"
                class="enter-receipt__input"
                type="number"
                inputmode="decimal"
                min="0"
                @input="validateNumberInput($event, index, 'Price')"
                @blur="validateField('Item', index, 'Price')"
              />
              <span
                v-if="errors[`Items.${index}.Price`]"
                class="enter-receipt__error"
              >
                {{ errors[`Items.${index}.Price`] }}
              </span>
            </div>

            <span>&times;</span>

            <div>
              <label for="quantity" class="enter-receipt__label">Кол-во</label>
              <input
                id="quantity"
                v-model.number="item.Quantity"
                class="enter-receipt__input"
                type="number"
                inputmode="decimal"
                min="0"
                @input="validateNumberInput($event, index, 'Quantity')"
                @blur="validateField('Item', index, 'Quantity')"
              />
              <span
                v-if="errors[`Items.${index}.Quantity`]"
                class="enter-receipt__error"
              >
                {{ errors[`Items.${index}.Quantity`] }}
              </span>
            </div>
          </div>
          <span>Итого: {{ item.Sum }} ₽</span>
        </div>
      </div>

      <div class="enter-receipt__buttons-group">
        <button
          class="enter-receipt__button enter-receipt__button--cancel"
          @click="$emit('close')"
        >
          Отмена
        </button>
        <button
          type="submit"
          class="enter-receipt__button enter-receipt__button--submit"
          :disabled="isLoading"
          @click="submitReceipt"
        >
          {{ isLoading ? "Сохранение..." : "Добавить" }}
        </button>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
  import { ref, watch } from "vue";
  import api from "@/api/axios";
  import { useReceipts } from "@/composables/useReceipts";
  import { useErrorHandler } from "@/composables/useErrorHandler";

  const { fetchReceipts } = useReceipts();
  const { errorMessage, handleApiError } = useErrorHandler();

  const errors = ref<{ [key: string]: string }>({});

  interface Item {
    Sum: number;
    Name: string;
    Price: number;
    Quantity: number;
  }

  interface Receipt {
    RetailPlace: string;
    DateTime: string;
    TotalSum: number;
    Items: Item[];
  }

  const receipt = ref<Receipt>({
    RetailPlace: "",
    DateTime: new Date().toISOString(),
    TotalSum: 0,
    Items: [{ Sum: 0, Name: "", Price: 0, Quantity: 1 }],
  });

  const isLoading = ref(false);
  const scanSuccess = ref(false);
  const successCount = ref(0);

  const validateField = (field: string, index?: number, itemField?: string) => {
    const minDate = new Date(2017, 6, 1);
    const maxDate = new Date();

    if (field === "RetailPlace") {
      if (!receipt.value.RetailPlace.trim()) {
        errors.value.RetailPlace = "Укажите название магазина";
      } else {
        delete errors.value.RetailPlace;
      }
    } else if (field === "DateTime") {
      if (!receipt.value.DateTime) {
        errors.value.DateTime = "Укажите дату чека";
      } else {
        const receiptDate = new Date(receipt.value.DateTime);
        if (receiptDate < minDate) {
          errors.value.DateTime = "Дата не может быть раньше 1 июля 2017 года";
        } else if (receiptDate > maxDate) {
          errors.value.DateTime = "Дата не может быть в будущем";
        } else {
          delete errors.value.DateTime;
        }
      }
    } else if (field === "Item" && typeof index === "number" && itemField) {
      const item = receipt.value.Items[index];

      const isEmptyLastItem =
        index === receipt.value.Items.length - 1 &&
        !item.Name.trim() &&
        item.Price === 0 &&
        item.Quantity === 1;

      if (receipt.value.Items.length === 1 || !isEmptyLastItem) {
        if (itemField === "Name") {
          const errorKey = `Items.${index}.Name`;
          if (!item.Name.trim()) {
            errors.value[errorKey] = "Введите название товара";
          } else {
            delete errors.value[errorKey];
          }
        } else if (itemField === "Price") {
          const errorKey = `Items.${index}.Price`;
          if (item.Price <= 0) {
            errors.value[errorKey] = "Цена должна быть больше 0";
          } else {
            delete errors.value[errorKey];
          }
        } else if (itemField === "Quantity") {
          const errorKey = `Items.${index}.Quantity`;
          if (item.Quantity <= 0) {
            errors.value[errorKey] = "Количество должно быть больше 0";
          } else {
            delete errors.value[errorKey];
          }
        }
      }
    }
  };

  const validateAllFields = () => {
    errorMessage.value = null;

    validateField("RetailPlace");
    validateField("DateTime");

    receipt.value.Items.forEach((item, index) => {
      validateField("Item", index, "Name");
      validateField("Item", index, "Price");
      validateField("Item", index, "Quantity");
    });

    const validItems = receipt.value.Items.filter(
      (item) => item.Name.trim() !== "" && item.Price > 0
    );

    if (validItems.length === 0) {
      errorMessage.value = "Добавьте хотя бы один товар";
      return false;
    }

    return Object.keys(errors.value).length === 0;
  };

  const submitReceipt = async () => {
    if (!validateAllFields()) {
      return;
    }

    try {
      isLoading.value = true;

      const validItems = receipt.value.Items.filter(
        (item) => item.Name.trim() !== "" && item.Price > 0
      );

      const itemsInKopeks = validItems.map((item) => ({
        Name: item.Name,
        Price: Math.round(item.Price * 100),
        Quantity: item.Quantity,
        Sum: Math.round(item.Sum * 100),
      }));

      const receiptToSend = {
        RetailPlace: receipt.value.RetailPlace,
        DateTime: receipt.value.DateTime,
        TotalSum: Math.round(receipt.value.TotalSum * 100),
        Items: itemsInKopeks,
      };

      await api.post("/receipt/manual-add", receiptToSend);
      await fetchReceipts();

      scanSuccess.value = true;
    } catch (error) {
      handleApiError(error);
    } finally {
      isLoading.value = false;
    }
  };

  const resetForm = () => {
    scanSuccess.value = false;
    successCount.value = 0;
    errorMessage.value = null;
    errors.value = {};

    receipt.value = {
      RetailPlace: "",
      DateTime: new Date().toISOString(),
      TotalSum: 0,
      Items: [{ Sum: 0, Name: "", Price: 0, Quantity: 1 }],
    };
  };

  const validateNumberInput = (
    event: Event,
    index: number,
    field: "Price" | "Quantity"
  ) => {
    const input = event.target as HTMLInputElement;

    if (parseFloat(input.value) < 0) {
      input.value = "0";
      receipt.value.Items[index][field] = 0;
    }

    const hasDecimal = input.value.includes(".") || input.value.includes(",");

    if (!hasDecimal) {
      receipt.value.Items[index][field] =
        input.value === "" ? 0 : parseFloat(input.value);
      handleInput(index);
      return;
    }

    if (input.value.includes(",")) {
      const parts = input.value.split(",");
      input.value = parts[0] + "." + parts.slice(1).join("");
    }

    const parts = input.value.split(".");
    if (parts.length > 2) {
      input.value = parts[0] + "." + parts.slice(1).join("");
    }

    if (field === "Price") {
      const cleanValue = parseFloat(input.value);
      const roundedValue = cleanValue ? Math.round(cleanValue * 100) / 100 : 0;

      receipt.value.Items[index][field] = roundedValue;

      if (!input.value.endsWith(".") && input.value !== parts[0] + ".") {
        input.value = roundedValue.toString();
      }
    } else if (field === "Quantity") {
      const cleanValue = parseFloat(input.value);
      const roundedValue = cleanValue ? Math.round(cleanValue * 1000) / 1000 : 0;

      receipt.value.Items[index][field] = roundedValue;

      if (!input.value.endsWith(".") && input.value !== parts[0] + ".") {
        input.value = roundedValue.toString();
      }
    }

    handleInput(index);
  };

  const updateTotalSum = () => {
    receipt.value.TotalSum =
      Math.round(
        receipt.value.Items.reduce((sum, item) => sum + item.Sum, 0) * 100
      ) / 100;
  };

  const handleInput = (index: number) => {
    const item = receipt.value.Items[index];

    if (!item.Name && !item.Price && !item.Quantity) return;

    if (index === receipt.value.Items.length - 1) {
      receipt.value.Items.push({ Sum: 0, Name: "", Price: 0, Quantity: 1 });
    }
  };

  const removeItem = (index: number) => {
    if (receipt.value.Items.length > 1) {
      receipt.value.Items.splice(index, 1);
      updateTotalSum();
    }
  };

  watch(
    () => receipt.value.Items,
    () => {
      receipt.value.Items.forEach((item) => {
        item.Sum = Math.round(item.Price * item.Quantity * 100) / 100;
      });
      updateTotalSum();
    },
    { deep: true }
  );
</script>

<style lang="scss" scoped>
  .enter-receipt {
    margin-top: 15px;

    &__container {
      display: flex;
      flex-direction: column;
    }

    &__header {
      display: flex;
      flex-direction: column;
      padding: 15px;
      border: 2px solid var(--primary-green);
      border-radius: 10px;
      background-color: rgba(137, 225, 89, 0.1);
      gap: 5px;
      margin-bottom: 10px;
    }

    &__label {
      font-size: 16px;
      font-weight: 600;
    }

    &__input {
      width: 100%;
      font-size: 16px;
      border-radius: 5px;
      box-sizing: border-box;
      padding: 8px 12px;
      border: 1px solid var(--primary-green);
      outline-offset: -2px;
      &--error {
        border: 1px solid var(--vt-c-light-red);
      }
    }

    &__totalsum {
      font-size: 20px;
      font-weight: 600;
    }

    &__line {
      width: 100%;
      height: 1px;
      background-color: var(--vt-c-blue-gray);
      margin: 15px 0;
    }

    &__items-list {
      overflow-y: scroll;
      overflow-x: hidden;
      max-height: 400px;
    }

    &__item {
      position: relative;
      padding: 15px;
      border: 2px solid var(--primary-green);
      border-radius: 10px;
      margin-bottom: 10px;

      & > span {
        font-size: 18px;
        font-weight: 600;
      }
    }

    &__remove-button {
      position: absolute;
      width: 32px;
      height: 32px;
      top: 5px;
      right: 5px;
      background: none;
      border: none;
      color: var(--vt-c-white);
      font-size: 24px;
      transition: 0.2s;

      @media (hover: hover) and (pointer: fine) {
        &:hover {
          color: var(--vt-c-light-red);
          transition: 0.2s;
        }
      }
    }

    &__error-message {
      margin-bottom: 10px;
      text-align: center;
      font-size: 16px;
      padding: 10px;
      border-radius: 10px;
      border: 2px solid var(--vt-c-light-red);
      background-color: rgba(255, 77, 77, 0.1);
      color: var(--vt-c-white);
    }

    &__error {
      font-size: 14px !important;
      font-weight: 500 !important;
      color: var(--vt-c-light-red);
    }

    &__inputs-group {
      display: flex;
      align-items: flex-start;
      gap: 10px;
      margin-bottom: 5px;

      & > span {
        font-size: 24px;
        font-weight: 600;
        align-self: center;
        margin-top: 1.5rem;
      }
    }

    &__success-container {
      display: flex;
      flex-direction: column;
      width: 100%;
      box-sizing: border-box;
    }

    &__success-screen {
      display: flex;
      flex-direction: column;
      align-items: center;
      justify-content: center;
      padding: 20px;
      background: var(--vt-c-dark-blue-gray);
      border-radius: 8px;
      margin-bottom: 15px;
      min-height: 300px;
    }

    &__success-icon {
      margin-bottom: 20px;
    }

    &__success-message {
      color: var(--vt-c-white);
      font-size: 1.125rem;
      font-weight: 500;
      text-align: center;
      margin: 0;
    }

    &__success-buttons {
      display: flex;
      gap: 10px;
      width: 100%;
    }

    &__buttons-group {
      display: flex;
      gap: 10px;
      margin-top: 20px;
    }

    &__button {
      flex: 1;
      padding: 8px 16px;
      border-radius: 4px;
      font-size: 16px;
      font-weight: 600;
      transition: 0.2s;
      cursor: pointer;

      &--submit {
        background-color: var(--primary-green);
        color: var(--vt-c-dark-blue-gray);
        border: 2px solid var(--primary-green);

        @media (hover: hover) and (pointer: fine) {
          &:hover {
            background: none;
            color: var(--primary-green);
            transition: 0.2s;
          }
        }
      }

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
    }
  }
</style>
