<template>
  <div class="auth">
    <div class="auth__card">
      <h2 class="auth__title">
        {{ isRegistering ? "Регистрация" : "Авторизация" }}
      </h2>
      <form @submit.prevent="handleSubmit" class="auth__form">
        <p v-if="errorMessage" class="auth__error">{{ errorMessage }}</p>
        <div class="auth__form-group">
          <label for="username" class="auth__label">Логин</label>
          <input
            type="text"
            id="username"
            v-model="formData.username"
            placeholder="Введите ваш логин"
            class="auth__input"
            required
          />
        </div>

        <div class="auth__form-group" v-if="isRegistering">
          <label for="email" class="auth__label">Email</label>
          <input
            type="email"
            id="email"
            v-model="formData.email"
            placeholder="Введите ваш email"
            class="auth__input"
            required
          />
        </div>

        <div class="auth__form-group">
          <label for="password" class="auth__label">Пароль</label>
          <input
            type="password"
            id="password"
            v-model="formData.password"
            placeholder="Введите ваш пароль"
            class="auth__input"
            required
          />
        </div>

        <button type="submit" class="auth__button">
          {{ isRegistering ? "Зарегистрироваться" : "Войти" }}
        </button>
      </form>
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
  import { ref } from "vue";
  import api from "@/api/axios";

  const isRegistering = ref(false);
  const formData = ref({
    username: "",
    email: "",
    password: "",
  });

  const errorMessage = ref("");

  function toggleForm() {
    isRegistering.value = !isRegistering.value;
    formData.value = { username: "", email: "", password: "" };
    errorMessage.value = "";
  }

  async function handleSubmit() {
    try {
      errorMessage.value = "";
      let response;

      if (isRegistering.value) {
        response = await api.post("/register", {
          username: formData.value.username,
          email: formData.value.email,
          password: formData.value.password,
        });
        console.log("Регистрация успешна:", response.data);
      } else {
        response = await api.post("/login", {
          username: formData.value.username,
          password: formData.value.password,
        });
        console.log(
          isRegistering.value ? "Регистрация успешна" : "Авторизация успешна",
          response.data
        );
      }
    } catch (error: any) {
      if (error.response) {
        errorMessage.value = error.response.data.message || "Произошла ошибка";
      } else {
        errorMessage.value = "Ошибка сети";
      }
    }
  }
</script>

<style lang="scss" scoped>
  .auth {
    &__error {
      color: #ff4d4d;
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
        cursor: pointer;
        font-size: 14px;
        padding: 0;
        text-decoration: underline;
        transition: 0.2s;

        @media (hover: hover) and (pointer: fine) {
          &:hover {
            color: var(--dark-green);
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
