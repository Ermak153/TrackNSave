<template>
  <div class="auth">
    <div class="auth__card">
      <h2 class="auth__title">
        {{ isRegistering ? "Регистрация" : "Авторизация" }}
      </h2>
      <Form
        :validation-schema="currentSchema"
        @submit="handleSubmit"
        class="auth__form"
        ref="form"
      >
        <div v-if="errorMessage" class="auth__error-message">
          {{ errorMessage }}
        </div>
        <div class="auth__form-group">
          <label for="username" class="auth__label">Логин</label>
          <Field name="username" v-slot="{ field, errorMessage }">
            <input
              v-bind="field"
              id="username"
              :class="['auth__input', { 'auth__input--error': errorMessage }]"
              placeholder="Введите ваш логин"
              v-model="formData.username"
            />
          </Field>
          <ErrorMessage name="username" v-slot="{ message }">
            <div v-if="message" class="auth__error">{{ message }}</div>
          </ErrorMessage>
        </div>

        <div class="auth__form-group" v-if="isRegistering">
          <label for="email" class="auth__label">Email</label>
          <Field name="email" v-slot="{ field, errorMessage }">
            <input
              v-bind="field"
              id="email"
              :class="['auth__input', { 'auth__input--error': errorMessage }]"
              placeholder="Введите ваш email"
              v-model="formData.email"
            />
          </Field>
          <ErrorMessage name="email" v-slot="{ message }">
            <div v-if="message" class="auth__error">{{ message }}</div>
          </ErrorMessage>
        </div>

        <div class="auth__form-group">
          <label for="password" class="auth__label">Пароль</label>
          <Field name="password" v-slot="{ field, errorMessage }">
            <input
              v-bind="field"
              id="password"
              type="password"
              :class="['auth__input', { 'auth__input--error': errorMessage }]"
              placeholder="Введите ваш пароль"
              v-model="formData.password"
            />
          </Field>
          <ErrorMessage name="password" v-slot="{ message }">
            <div v-if="message" class="auth__error">{{ message }}</div>
          </ErrorMessage>
        </div>

        <button type="submit" class="auth__button">
          {{ isRegistering ? "Зарегистрироваться" : "Войти" }}
        </button>
      </Form>

      <p class="auth__toggle">
        {{ isRegistering ? "Уже есть аккаунт?" : "Нет аккаунта?" }}
        <button @click="toggleForm" class="auth__toggle-button">
          {{ isRegistering ? "Войти" : "Зарегистрироваться" }}
        </button>
      </p>
    </div>
  </div>
</template>

<script setup lang="ts">
  import api from "@/api/axios";
  import { string, object } from "yup";
  import { AxiosError } from "axios";
  import { Form, Field, ErrorMessage, configure } from "vee-validate";
  import type { FormActions } from "vee-validate";
  import { ref, computed } from "vue";
  import { useAuth } from "@/composables/useAuth.ts";
  import router from "@/router";

  const { checkAuthStatus } = useAuth();

  configure({
    validateOnBlur: true,
    validateOnChange: true,
    validateOnInput: false,
    validateOnModelUpdate: false,
  });

  interface FormValues {
    username: string;
    email?: string;
    password: string;
  }

  const form = ref<FormActions<FormValues>>();

  const loginSchema = object({
    username: string()
      .required("Логин обязателен")
      .matches(/^[a-zA-Z0-9]+$/, "Допускаются только латинские буквы и цифры")
      .min(4, "Логин должен быть длиннее 4 символов")
      .max(20, "Логин не должен быть короче 20 символов"),
    password: string()
      .required("Пароль обязателен")
      .min(5, "Пароль должен быть длиннее 5 символов"),
  });

  const registerSchema = object({
    username: string()
      .required("Логин обязателен")
      .matches(/^[a-zA-Z0-9]+$/, "Допускаются только латинские буквы и цифры")
      .min(4, "Логин должен быть длиннее 4 символов")
      .max(20, "Логин не должен быть короче 20 символов"),
    email: string()
      .required("Email обязателен")
      .email("Введите корректный email"),
    password: string()
      .required("Пароль обязателен")
      .min(5, "Пароль должен быть длиннее 5 символов"),
  });

  const isRegistering = ref(false);
  const formData = ref({
    username: "",
    email: "",
    password: "",
  });

  const errorMessage = ref("");

  const currentSchema = computed(() => {
    return isRegistering.value ? registerSchema : loginSchema;
  });

  function toggleForm() {
    isRegistering.value = !isRegistering.value;
    formData.value = { username: "", email: "", password: "" };
    errorMessage.value = "";

    if (form.value) {
      form.value.resetForm();
    }
  }

  async function handleSubmit() {
    try {
      errorMessage.value = "";

      if (isRegistering.value) {
        await api.post("/auth/register", {
          username: formData.value.username,
          email: formData.value.email,
          password: formData.value.password,
        });
        await api.post("/auth/login", {
          username: formData.value.username,
          password: formData.value.password,
        });
        await checkAuthStatus();
        router.push("/");
      } else {
        await api.post("/auth/login", {
          username: formData.value.username,
          password: formData.value.password,
        });
        await checkAuthStatus();
        router.push("/");
      }
    } catch (error: unknown) {
      if (error instanceof AxiosError) {
        if (!error.response) {
          errorMessage.value = "Ошибка сети. Проверьте подключение.";
        } else {
          const status = error.response.status;
          const message = error.response.data?.message || "Произошла ошибка";
          switch (status) {
            case 401:
              errorMessage.value = `Неверное имя пользователя или пароль`;
              break;

            case 409:
              switch (message) {
                case "Username already taken":
                  errorMessage.value = "Имя пользователя занято";
                  break;

                case "Email already taken":
                  errorMessage.value = "Этот E-mail уже зарегистрирован";
                  break;
              }
              break;
          }
        }
      } else {
        errorMessage.value = "Непредвиденная ошибка";
      }
    }
  }
</script>

<style lang="scss" scoped>
  .auth {
    &__error {
      color: var(--vt-c-light-red);
      font-size: 14px;
    }

    &__error-message {
      color: var(--vt-c-light-red);
      background: rgba(255, 77, 77, 0.1);
      padding: 8px;
      border-radius: 5px;
      text-align: center;
      margin-bottom: 10px;
      font-size: 14px;
    }

    &__card {
      background: linear-gradient(
        301.8deg,
        #1e4e4e 16.05%,
        #1f2431 75.16%,
        #1f2431
      );
      padding: 20px 30px;
      border-radius: 10px;
      box-shadow: 0 4px 10px rgba(0, 0, 0, 0.1);
      max-width: 400px;
      width: 100%;
      text-align: center;
    }

    &__title {
      margin-bottom: 20px;
      color: var(--vt-c-white);
    }

    &__form {
      margin-bottom: 15px;
      text-align: left;
    }

    &__form-group {
      margin-bottom: 15px;
      text-align: left;

      &--confirm {
        margin-top: 10px;
      }
    }

    &__label {
      display: block;
      margin-bottom: 5px;
      color: var(--vt-c-white);
    }

    &__input {
      width: 100%;
      padding: 8px 12px;
      border: 1px solid var(--primary-green);
      border-radius: 5px;
      font-size: 16px;
      box-sizing: border-box;
      transition: border-color 0.2s ease;

      &--error {
        border-color: var(--vt-c-light-red);
      }
    }

    &__button {
      background: var(--primary-green);
      color: var(--vt-c-white);
      border: none;
      padding: 10px 15px;
      border-radius: 5px;
      border: 2px solid transparent;
      cursor: pointer;
      font-size: 16px;
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

    &__toggle {
      margin-top: 10px;
      color: var(--vt-c-white);

      &-button {
        background: none;
        border: none;
        color: var(--primary-green);
        font-size: 14px;
        padding: 0;
        text-decoration: underline;
        transition: 0.2s;

        @media (hover: hover) and (pointer: fine) {
          &:hover {
            color: var(--primary-green);
            opacity: 0.6;
            transition: 0.2s;
          }
        }
      }
    }

    @media (max-width: 768px) {
      &__card {
        padding: 15px 20px;
        max-width: 90%;
        margin: 0 15px;
      }

      &__title {
        font-size: 20px;
        margin-bottom: 15px;
      }

      &__input {
        padding: 6px 10px;
        font-size: 16px;
      }

      &__button {
        font-size: 14px;
        padding: 8px 12px;
      }

      &__toggle-button {
        font-size: 14px;
      }
    }
  }
</style>
