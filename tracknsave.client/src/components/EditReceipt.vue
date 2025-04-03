<template>
  <Teleport to="body">
    <Transition name="modal">
      <div v-if="modelValue" class="modal__overlay" @click.self="close">
        <div class="modal">
          <header class="modal__header">
            <button class="modal__close" @click="close">&times;</button>
          </header>

          <div class="modal__body">
            <div class="modal__content">
              <div class="enter-receipt">
                <div class="enter-receipt__container">
                  <span
                    class="enter-receipt__error-message"
                    v-if="errorMessage"
                    >{{ errorMessage }}</span
                  >
                  <span class="enter-receipt__totalsum"
                    >Итоговая сумма: {{ receiptData.TotalSum }} ₽</span
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
                          v-model="receiptData.RetailPlace"
                          placeholder="Название магазина"
                          class="enter-receipt__input"
                          @blur="validateField('RetailPlace')"
                        />
                        <span
                          v-if="errors.RetailPlace"
                          class="enter-receipt__error"
                          >{{ errors.RetailPlace }}</span
                        >
                      </div>
                      <div>
                        <label for="datetime" class="enter-receipt__label"
                          >Время покупки</label
                        >
                        <input
                          v-model="receiptData.DateTime"
                          id="datetime"
                          type="datetime-local"
                          class="enter-receipt__input"
                          @blur="validateField('DateTime')"
                        />
                        <span
                          v-if="errors.DateTime"
                          class="enter-receipt__error"
                          >{{ errors.DateTime }}</span
                        >
                      </div>
                    </div>

                    <div class="enter-receipt__line"></div>

                    <div
                      v-for="(item, index) in receiptData.Items"
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
                          <label for="price" class="enter-receipt__label"
                            >Цена</label
                          >
                          <input
                            id="price"
                            v-model="item.Price"
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
                          <label for="quantity" class="enter-receipt__label"
                            >Кол-во</label
                          >
                          <input
                            id="quantity"
                            v-model="item.Quantity"
                            class="enter-receipt__input"
                            type="number"
                            inputmode="decimal"
                            min="0"
                            @input="
                              validateNumberInput($event, index, 'Quantity')
                            "
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
                      @click="cancel"
                    >
                      Отмена
                    </button>
                    <button
                      type="submit"
                      class="enter-receipt__button enter-receipt__button--submit"
                      :disabled="isLoading"
                      @click="submitReceipt"
                    >
                      {{ isLoading ? "Сохранение..." : "Изменить" }}
                    </button>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </Transition>
  </Teleport>
</template>

<script setup lang="ts">
  import { ref, watch } from "vue";
  import api from "@/api/axios";
  import { useReceipts } from "@/composables/useReceipts";
  import { useErrorHandler } from "@/composables/useErrorHandler";
  import { useToast } from "@/composables/useToast";

  const toast = useToast();

  const { fetchReceipts } = useReceipts();
  const { errorMessage, handleApiError } = useErrorHandler();

  const props = defineProps<{
    modelValue: boolean;
    receiptId: number;
    receipt?: {
      RetailPlace: string;
      DateTime: string;
      TotalSum: number;
      Items: {
        Sum: number;
        Name: string;
        Price: number;
        Quantity: number;
      }[];
    };
  }>();

  const emit = defineEmits<{
    (e: "update:modelValue", value: boolean): void;
    (e: "close"): void;
  }>();

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

  const receiptData = ref<Receipt>({
    RetailPlace: "",
    DateTime: new Date().toISOString(),
    TotalSum: 0,
    Items: [{ Sum: 0, Name: "", Price: 0, Quantity: 1 }],
  });

  const isLoading = ref(false);

  watch(
    () => props.modelValue,
    (newValue) => {
      if (newValue && props.receipt) {
        receiptData.value = {
          RetailPlace: props.receipt.RetailPlace,
          DateTime: props.receipt.DateTime,
          TotalSum: props.receipt.TotalSum / 100,
          Items: props.receipt.Items.map((item) => ({
            ...item,
            Price: item.Price / 100,
            Sum: item.Sum / 100,
          })),
        };
      }
    }
  );

  const close = () => {
    emit("update:modelValue", false);
    emit("close");
  };

  const cancel = () => {
    emit("update:modelValue", false);
    emit("close");
    toast.show("Изменение отменено", "info");
  };

  const validateField = (field: string, index?: number, itemField?: string) => {
    const minDate = new Date(2017, 6, 1);
    const maxDate = new Date();

    if (field === "RetailPlace") {
      if (!receiptData.value.RetailPlace.trim()) {
        errors.value.RetailPlace = "Укажите название магазина";
      } else {
        delete errors.value.RetailPlace;
      }
    } else if (field === "DateTime") {
      if (!receiptData.value.DateTime) {
        errors.value.DateTime = "Укажите дату чека";
      } else {
        const receiptDate = new Date(receiptData.value.DateTime);
        if (receiptDate < minDate) {
          errors.value.DateTime = "Дата не может быть раньше 1 июля 2017 года";
        } else if (receiptDate > maxDate) {
          errors.value.DateTime = "Дата не может быть в будущем";
        } else {
          delete errors.value.DateTime;
        }
      }
    } else if (field === "Item" && typeof index === "number" && itemField) {
      const item = receiptData.value.Items[index];

      const isEmptyLastItem =
        index === receiptData.value.Items.length - 1 &&
        !item.Name.trim() &&
        item.Price === 0 &&
        item.Quantity === 1;

      if (receiptData.value.Items.length === 1 || !isEmptyLastItem) {
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

    receiptData.value.Items.forEach((item, index) => {
      validateField("Item", index, "Name");
      validateField("Item", index, "Price");
      validateField("Item", index, "Quantity");
    });

    const validItems = receiptData.value.Items.filter(
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

      const validItems = receiptData.value.Items.filter(
        (item) => item.Name.trim() !== "" && item.Price > 0
      );

      const itemsInKopeks = validItems.map((item) => ({
        Name: item.Name,
        Price: Math.round(item.Price * 100),
        Quantity: item.Quantity,
        Sum: Math.round(item.Sum * 100),
      }));

      const receiptToSend = {
        ReceiptId: props.receiptId,
        Receipt: {
          RetailPlace: receiptData.value.RetailPlace,
          DateTime: receiptData.value.DateTime,
          TotalSum: Math.round(receiptData.value.TotalSum * 100),
          Items: itemsInKopeks,
        },
      };

      await api.put("/receipt/edit", receiptToSend);
      await fetchReceipts();

      toast.show("Чек успешно изменен", "success");
      close();
    } catch (error) {
      handleApiError(error);
    } finally {
      isLoading.value = false;
    }
  };

  const validateNumberInput = (
    event: Event,
    index: number,
    field: "Price" | "Quantity"
  ) => {
    const input = event.target as HTMLInputElement;

    if (parseFloat(input.value) < 0) {
      input.value = "0";
      receiptData.value.Items[index][field] = 0;
    }

    const hasDecimal = input.value.includes(".") || input.value.includes(",");

    if (!hasDecimal) {
      receiptData.value.Items[index][field] =
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

      receiptData.value.Items[index][field] = roundedValue;

      if (!input.value.endsWith(".") && input.value !== parts[0] + ".") {
        input.value = roundedValue.toString();
      }
    } else if (field === "Quantity") {
      const cleanValue = parseFloat(input.value);
      const roundedValue = cleanValue ? Math.round(cleanValue * 1000) / 1000 : 0;

      receiptData.value.Items[index][field] = roundedValue;

      if (!input.value.endsWith(".") && input.value !== parts[0] + ".") {
        input.value = roundedValue.toString();
      }
    }

    handleInput(index);
  };

  const updateTotalSum = () => {
    receiptData.value.TotalSum =
      Math.round(
        receiptData.value.Items.reduce((sum, item) => sum + item.Sum, 0) * 100
      ) / 100;
  };

  const handleInput = (index: number) => {
    const item = receiptData.value.Items[index];

    if (!item.Name && !item.Price && !item.Quantity) return;

    if (index === receiptData.value.Items.length - 1) {
      receiptData.value.Items.push({ Sum: 0, Name: "", Price: 0, Quantity: 1 });
    }
  };

  const removeItem = (index: number) => {
    if (receiptData.value.Items.length > 1) {
      receiptData.value.Items.splice(index, 1);
      updateTotalSum();
    }
  };

  watch(
    () => receiptData.value.Items,
    () => {
      receiptData.value.Items.forEach((item) => {
        item.Sum = Math.round(item.Price * item.Quantity * 100) / 100;
      });
      updateTotalSum();
    },
    { deep: true }
  );
</script>

<style lang="scss" scoped>
  .modal {
    display: flex;
    flex-direction: column;
    justify-content: flex-start;
    background: var(--vt-c-dark-blue-gray);
    color: var(--vt-c-white);
    padding: 20px;
    border-radius: 12px;
    box-shadow: 0 10px 30px rgba(0, 0, 0, 0.3);
    max-height: 85vh;
    max-width: 80vw;
    margin-top: 5vh;

    &__overlay {
      position: fixed;
      inset: 0;
      background: rgba(0, 0, 0, 0.6);
      display: flex;
      align-items: flex-start;
      justify-content: center;
      z-index: 1000;
      height: 100vh;
      width: 100vw;
      padding-top: 5vh;
    }

    &__header {
      display: flex;
      justify-content: flex-end;
      margin-bottom: 10px;
    }

    &__close {
      background: none;
      border: none;
      font-size: 30px;
      color: inherit;
      font-weight: 500;
      transition: 0.2s;
      cursor: pointer;

      @media (hover: hover) and (pointer: fine) {
        &:hover {
          color: var(--vt-c-light-red);
          transition: 0.2s;
        }
      }
    }

    &__body {
      display: flex;
      flex-direction: column;
      flex-grow: 1;
      min-height: 0;
    }

    &__content {
      width: min(400px, 80vw);
      display: flex;
      flex-direction: column;
      overflow: hidden;
    }
  }

  .enter-receipt {
    margin-top: 0;

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

  .modal-enter-active,
  .modal-leave-active {
    transition: opacity 0.3s ease;
  }

  .modal-enter-from,
  .modal-leave-to {
    opacity: 0;
  }
</style>
