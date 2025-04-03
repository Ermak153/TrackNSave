<template>
  <div class="receipt-form">
    <div v-if="scanSuccess" class="receipt-form__success-container">
      <div class="receipt-form__success-screen">
        <div class="receipt-form__success-icon">
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
        <p class="receipt-form__success-message">Чек успешно добавлен</p>
      </div>

      <div class="receipt-form__success-buttons">
        <button
          class="receipt-form__button receipt-form__button--close"
          @click="$emit('close')"
        >
          Закрыть
        </button>
        <button
          class="receipt-form__button receipt-form__button--add-more"
          @click="resetForm"
        >
          Добавить ещё
        </button>
      </div>
    </div>

    <Form
      v-if="!scanSuccess"
      :validation-schema="fiscalSchema"
      @submit="handleSubmit"
      ref="form"
    >
      <div class="receipt-form__group">
        <label for="fn" class="receipt-form__label">ФН</label>
        <Field name="fn" v-slot="{ field, errorMessage }">
          <input
            v-bind="field"
            id="fn"
            :class="[
              'receipt-form__input',
              { 'receipt-form__input--error': errorMessage },
            ]"
            placeholder="Номер фискального накопителя"
            maxlength="16"
            v-model="formData.fn"
          />
        </Field>
        <ErrorMessage name="fn" v-slot="{ message }">
          <div v-if="message" class="receipt-form__error">{{ message }}</div>
        </ErrorMessage>
      </div>

      <div class="receipt-form__group">
        <label for="fd" class="receipt-form__label">ФД</label>
        <Field name="fd" v-slot="{ field, errorMessage }">
          <input
            v-bind="field"
            id="fd"
            :class="[
              'receipt-form__input',
              { 'receipt-form__input--error': errorMessage },
            ]"
            placeholder="Номер фискального документа"
            maxlength="10"
            v-model="formData.fd"
          />
        </Field>
        <ErrorMessage name="fd" v-slot="{ message }">
          <div v-if="message" class="receipt-form__error">{{ message }}</div>
        </ErrorMessage>
      </div>

      <div class="receipt-form__group">
        <label for="fp" class="receipt-form__label">ФП</label>
        <Field name="fp" v-slot="{ field, errorMessage }">
          <input
            v-bind="field"
            id="fp"
            :class="[
              'receipt-form__input',
              { 'receipt-form__input--error': errorMessage },
            ]"
            placeholder="Фискальный признак документа"
            maxlength="10"
            v-model="formData.fp"
          />
        </Field>
        <ErrorMessage name="fp" v-slot="{ message }">
          <div v-if="message" class="receipt-form__error">{{ message }}</div>
        </ErrorMessage>
      </div>

      <div class="receipt-form__group">
        <label for="s" class="receipt-form__label">Итог</label>
        <Field name="s" v-slot="{ field, errorMessage }">
          <input
            v-bind="field"
            id="s"
            :class="[
              'receipt-form__input',
              { 'receipt-form__input--error': errorMessage },
            ]"
            placeholder="Сумма чека"
            v-model="formData.s"
          />
        </Field>
        <ErrorMessage name="s" v-slot="{ message }">
          <div v-if="message" class="receipt-form__error">{{ message }}</div>
        </ErrorMessage>
      </div>

      <div class="receipt-form__datetime">
        <div class="receipt-form__group">
          <label for="date" class="receipt-form__label">Дата</label>
          <Field name="date" v-slot="{ field, errorMessage }">
            <input
              v-bind="field"
              id="date"
              :class="[
                'receipt-form__input',
                { 'receipt-form__input--error': errorMessage },
              ]"
              type="date"
              v-model="formData.date"
              @change="handleDateChange"
            />
          </Field>
          <ErrorMessage name="date" v-slot="{ message }">
            <div v-if="message" class="receipt-form__error">{{ message }}</div>
          </ErrorMessage>
        </div>

        <div class="receipt-form__group">
          <label for="time" class="receipt-form__label">Время</label>
          <Field name="time" v-slot="{ field, errorMessage }">
            <input
              v-bind="field"
              id="time"
              :class="[
                'receipt-form__input',
                { 'receipt-form__input--error': errorMessage },
              ]"
              type="time"
              v-model="formData.time"
              ref="timeInput"
            />
          </Field>
          <ErrorMessage name="time" v-slot="{ message }">
            <div v-if="message" class="receipt-form__error">{{ message }}</div>
          </ErrorMessage>
        </div>
      </div>

      <div class="receipt-form__group">
        <label for="n" class="receipt-form__label">Тип чека</label>
        <Field name="n" v-slot="{ field, errorMessage }">
          <select
            v-bind="field"
            id="n"
            :class="[
              'receipt-form__input',
              { 'receipt-form__input--error': errorMessage },
            ]"
            v-model="formData.n"
          >
            <option value="income" selected>Приход</option>
            <option value="return_income">Возврат прихода</option>
            <option value="expense">Расход</option>
            <option value="return_expense">Возврат расхода</option>
          </select>
        </Field>
        <ErrorMessage name="n" v-slot="{ message }">
          <div v-if="message" class="receipt-form__error">{{ message }}</div>
        </ErrorMessage>
      </div>

      <p v-if="errorMessage" class="receipt-form__error-message">
        {{ errorMessage }}
      </p>

      <div class="receipt-form__buttons-group">
        <button
          class="receipt-form__button receipt-form__button--cancel"
          @click="$emit('close')"
        >
          Отмена
        </button>
        <button
          type="submit"
          class="receipt-form__button receipt-form__button--submit"
          :disabled="isLoading"
        >
          {{ isLoading ? "Отправка..." : "Сохранить" }}
        </button>
      </div>
    </Form>
  </div>
</template>

<script setup lang="ts">
  import { Form, Field, ErrorMessage, configure } from "vee-validate";
  import type { FormActions } from "vee-validate";
  import { ref, onMounted } from "vue";
  import { string, date } from "yup";
  import api from "@/api/axios";
  import { useReceipts } from "@/composables/useReceipts";
  import { useErrorHandler } from "@/composables/useErrorHandler";

  const { errorMessage, handleApiError } = useErrorHandler();
  const { fetchReceipts } = useReceipts();

  const form = ref<FormActions<FormValues>>();
  const isLoading = ref(false);
  const scanSuccess = ref(false);
  const timeInput = ref<HTMLInputElement | null>(null);
  const previousDate = ref("");
  const formHeight = ref<number | null>(null);

  const fiscalSchema = {
    fn: string()
      .required("Номер фискального накопителя обязателен")
      .matches(/^[0-9]+$/, "Поле должно содержать только цифры")
      .min(16, "Номер должен содержать 16 цифр")
      .max(16, "Номер должен содержать 16 цифр"),
    fd: string()
      .required("Номер фискального документа обязателен")
      .matches(/^[0-9]+$/, "Поле должно содержать только цифры")
      .max(10, "Номер должен содержать не более 10 цифр"),
    fp: string()
      .required("Фискальный признак документа обязателен")
      .matches(/^[0-9]+$/, "Поле должно содержать только цифры")
      .max(10, "Номер должен содержать не более 10 цифр"),
    s: string()
      .required("Сумма чека обязательна")
      .matches(/^[0-9]+([.,][0-9]{0,2})?$/, "Сумма должна содержать только цифры"),
    date: date()
      .required("Дата обязательна")
      .min(new Date("2019-01-01"), "Дата не может быть старше 01.01.2019")
      .max(new Date(), "Дата не может быть в будущем")
      .typeError("Некорректный формат даты"),
    time: string().required("Время обязательно"),
  };

  interface FormValues {
    fn: string;
    fd: string;
    fp: string;
    s: string;
    date: string;
    time: string;
    n: string;
  }

  const formData = ref({
    fn: "",
    fd: "",
    fp: "",
    s: "",
    date: "",
    time: "",
    n: "income",
  });

  configure({
    validateOnBlur: true,
    validateOnChange: true,
    validateOnInput: false,
    validateOnModelUpdate: false,
  });

  onMounted(() => {
    setTimeout(() => {
      const formElement = document.querySelector(".receipt-form");
      if (formElement) {
        formHeight.value = formElement.clientHeight;
      }
    }, 100);
  });

  const handleDateChange = (event: Event) => {
    const input = event.target as HTMLInputElement;
    const currentValue = input.value;

    if (currentValue && currentValue.length === 10) {
      const currentYear = currentValue.substring(0, 4);
      const previousYear = previousDate.value.substring(0, 4);

      if (currentYear !== previousYear && currentYear.length === 4) {
        setTimeout(() => {
          if (timeInput.value) {
            timeInput.value.focus();
          }
        }, 50);
      }
    }

    previousDate.value = currentValue;
  };

  const formatDateTime = (dateStr: string, timeStr: string): string => {
    if (!dateStr || !timeStr) return "";

    const formattedDate = dateStr.replace(/-/g, "");
    const formattedTime = timeStr.replace(":", "");

    return `${formattedDate}T${formattedTime}`;
  };

  const generateReceiptString = (data: typeof formData.value): string => {
    const formattedDateTime = formatDateTime(data.date, data.time);

    return `t=${formattedDateTime}&s=${data.s}&fn=${data.fn}&i=${data.fd}&fp=${
      data.fp
    }&n=${
      data.n === "income"
        ? "1"
        : data.n === "return_income"
        ? "2"
        : data.n === "expense"
        ? "3"
        : "4"
    }`;
  };

  const resetForm = () => {
    scanSuccess.value = false;
    errorMessage.value = null;

    formData.value = {
      fn: "",
      fd: "",
      fp: "",
      s: "",
      date: "",
      time: "",
      n: "income",
    };

    if (form.value) {
      form.value.resetForm();
    }
  };

  const handleSubmit = async () => {
    try {
      isLoading.value = true;
      errorMessage.value = null;

      const formElement = document.querySelector(".receipt-form");
      if (formElement) {
        formHeight.value = formElement.clientHeight;
      }

      const receiptRaw = generateReceiptString(formData.value);

      await api.post("/receipt/add", { ReceiptRaw: receiptRaw });
      await fetchReceipts();
      scanSuccess.value = true;

    } catch (error) {
      handleApiError(error);
    } finally {
      isLoading.value = false;
    }
  };
</script>

<style lang="scss" scoped>
  .receipt-form {
    position: relative;
    width: 100%;

    &__group {
      display: flex;
      flex: 1;
      flex-direction: column;
      box-sizing: border-box;
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

    &__error {
      font-size: 14px;
      color: var(--vt-c-light-red);
    }

    &__error-message {
      color: var(--vt-c-light-red);
      margin: 0 0 10px;
      font-size: 0.875rem;
    }

    &__datetime {
      display: flex;
      justify-content: space-between;
      gap: 16px;
    }

    &__buttons-group {
      display: flex;
      gap: 10px;
      margin-top: 10px;
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

      &--close {
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

      &--add-more {
        background: var(--primary-green);
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
  }
</style>
