<template>
  <div class="profile">
    <div class="profile__container">
      <div class="profile__header">
        <div class="profile__avatar">
          <div class="profile__avatar-inner" @click="triggerFileInput">
            <img :src="avatarUrl || defaultAvatar" alt="Аватар пользователя" class="profile__avatar-image" />
            <input
              ref="fileInput"
              type="file"
              accept="image/*"
              class="profile__avatar-input"
              @change="handleFileSelect"
            />
            <div class="profile__avatar-overlay">
              <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
                <path d="M21 15v4a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2v-4"></path>
                <polyline points="17 8 12 3 7 8"></polyline>
                <line x1="12" y1="3" x2="12" y2="15"></line>
              </svg>
            </div>
          </div>

          <div v-if="avatarUrl" class="profile__avatar-actions">
            <button class="profile__avatar-delete" @click="showDeleteConfirm = true">
              <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
                <polyline points="3 6 5 6 21 6"></polyline>
                <path d="M19 6v14a2 2 0 0 1-2 2H7a2 2 0 0 1-2-2V6m3 0V4a2 2 0 0 1 2-2h4a2 2 0 0 1 2 2v2"></path>
                <line x1="10" y1="11" x2="10" y2="17"></line>
                <line x1="14" y1="11" x2="14" y2="17"></line>
              </svg>
              <span>Удалить</span>
            </button>
          </div>

          <p v-if="uploadError" class="profile__avatar-error">{{ uploadError }}</p>
        </div>
        <div class="profile__info">
          <h1 class="profile__name">{{ username }}</h1>
          <p class="profile__email">{{ email }}</p>
        </div>
      </div>

      <div class="profile__stats">
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
            <p class="info-card__label">Всего чеков</p>
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

        <div class="info-card">
          <div class="info-card__icon-wrapper">
            <svg class="info-card__icon" xmlns="http://www.w3.org/2000/svg" width="24" height="24"
                viewBox="0 0 24 24" fill="none" stroke="currentColor"
                stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
              <polygon points="12 2 15 8.5 22 9.3 17 14 18.5 21 12 17.5 5.5 21 7 14 2 9.3 9 8.5 12 2"/>
            </svg>
          </div>
          <div class="info-card__details">
            <h3 class="info-card__value">{{ mostPopularCategory }}</h3>
            <p class="info-card__label">Популярная категория</p>
          </div>
        </div>
      </div>

      <div class="profile__sections">
        <div class="profile__section">
          <h2 class="profile__section-title">Личная информация</h2>
          <div class="profile__info-grid">
            <div class="profile__info-item">
              <label class="profile__info-label">Логин</label>
              <p class="profile__info-value">{{ username }}</p>
            </div>
            <div class="profile__info-item">
              <label class="profile__info-label">В приложении</label>
              <p class="profile__info-value">{{ registrationTime }}</p>
            </div>
            <div class="profile__info-item">
              <label class="profile__info-label">Email</label>
              <p class="profile__info-value">{{ email }}</p>
            </div>
            <div class="profile__info-item">
              <label class="profile__info-label">Смена пароля</label>
              <p class="profile__info-value"><button class="profile__change-password" @click="showPasswordChange = true">Сменить пароль</button></p>
            </div>
          </div>
        </div>
      </div>
    </div>

    <div v-if="showCropper" class="profile__modal">
      <div class="profile__modal-container">
        <div class="profile__modal-header">
          <h3 class="profile__modal-title">Обрезать изображение</h3>
          <button class="profile__modal-close" @click="cancelCrop">
            <svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
              <line x1="18" y1="6" x2="6" y2="18"></line>
              <line x1="6" y1="6" x2="18" y2="18"></line>
            </svg>
          </button>
        </div>
        <Cropper
          ref="cropperRef"
          :src="imageSrc"
          class="cropper"
          image-restriction="stencil"
          :stencil-component="CircleStencil"
          :stencil-props="{
            aspectRatio: 1/1,
          }"
          :min-width="256"
          :background-wrapper-component="CustomBackgroundWrapper"
        />
        <div class="profile__modal-actions">
          <button @click="cancelCrop" class="profile__modal-button profile__modal-button--cancel">
            Отмена
          </button>
          <button @click="cropImage" class="profile__modal-button profile__modal-button--confirm">
            Применить
          </button>
        </div>
      </div>
    </div>

        <transition name="fade">
        <div v-if="showPasswordChange" class="delete-modal-overlay">
          <div class="delete-modal" @click.stop>
            <div class="delete-modal__content">
              <h3 class="delete-modal__title">Смена пароля</h3>

            <Form :validation-schema="currentSchema" @submit="handlePasswordChange" class="profile__password-form" ref="form">
            <div class="profile__form-group">
              <label for="currentPassword" class="profile__form-label">Текущий пароль</label>
              <Field name="currentPassword" v-slot="{ field, errorMessage }">
                <input
                  v-model="passwordForm.currentPassword"
                  v-bind="field"
                  id="currentPassword"
                  type="password"
                  :class="['profile__form-input', { 'profile__form-input--error': errorMessage }]"
                  required
                  placeholder="Введите текущий пароль"
                />
              </Field>
              <ErrorMessage name="currentPassword" v-slot="{ message }">
                <div v-if="message" class="profile__password-error">{{ message }}</div>
              </ErrorMessage>
            </div>

            <div class="profile__form-group">
              <label for="newPassword" class="profile__form-label">Новый пароль</label>
              <Field name="newPassword" v-slot="{ field, errorMessage }">
                <input
                  v-model="passwordForm.newPassword"
                  v-bind="field"
                  id="newPassword"
                  type="password"
                  :class="['profile__form-input', { 'profile__form-input--error': errorMessage }]"
                  required
                  placeholder="Введите новый пароль"
                />
              </Field>
              <ErrorMessage name="newPassword" v-slot="{ message }">
                <div v-if="message" class="profile__password-error">{{ message }}</div>
              </ErrorMessage>
            </div>

            <div class="profile__form-group">
              <label for="confirmPassword" class="profile__form-label">Подтверждение пароля</label>
              <Field name="confirmPassword" v-slot="{ field, errorMessage }">
                <input
                  v-model="passwordForm.confirmPassword"
                  v-bind="field"
                  id="confirmPassword"
                  type="password"
                  :class="['profile__form-input', { 'profile__form-input--error': errorMessage }]"
                  required
                  placeholder="Введите подтверждение пароля"
                />
              </Field>
              <ErrorMessage name="confirmPassword" v-slot="{ message }">
                <div v-if="message" class="profile__password-error">{{ message }}</div>
              </ErrorMessage>
            </div>

            <div v-if="passwordError" class="profile__form-message profile__form-message--error">
              {{ passwordError }}
            </div>

            <div v-if="passwordSuccess" class="profile__form-message profile__form-message--success">
              {{ passwordSuccess }}
            </div>

            <div class="delete-modal__actions">

              <button class="delete-modal__button delete-modal__button--cancel" @click="showPasswordChange = false"><span>Отмена</span></button>
              <button type="submit" class="profile__form-submit" :disabled="isChangingPassword">
                <span v-if="!isChangingPassword">Сменить пароль</span>
                <span v-else>Меняем пароль...</span>
              </button>
            </div>
          </Form>
            </div>
          </div>
        </div>
      </transition>

    <transition name="fade">
        <div v-if="showDeleteConfirm" class="delete-modal-overlay">
          <div class="delete-modal" @click.stop>
            <div class="delete-modal__content">
              <h3 class="delete-modal__title">Подтверждение удаления</h3>
              <p class="delete-modal__text">
                Вы уверены, что хотите удалить аватар?
              </p>

              <div class="delete-modal__actions">
                <button
                  class="delete-modal__button delete-modal__button--cancel"
                  @click="showDeleteConfirm = false"
                >
                  Отмена
                </button>
                <button
                  class="delete-modal__button delete-modal__button--confirm"
                  @click="confirmDeleteAvatar"
                >
                  Удалить
                </button>
              </div>
            </div>
          </div>
        </div>
      </transition>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { Cropper } from 'vue-advanced-cropper'
import 'vue-advanced-cropper/dist/style.css'
import { useUsers } from '@/composables/useUsers'
import CustomBackgroundWrapper from '@/components/CustomBackgroundWrapper.vue'
import CircleStencil from '@/components/CircleStencil.vue'
import defaultAvatar from "@/assets/defaultAvatar.png"
import { useErrorHandler } from '@/composables/useErrorHandler'
import { string } from "yup";
import { Form, Field, ErrorMessage, configure } from "vee-validate";
import type { FormActions } from "vee-validate";
import { useReceipts } from '@/composables/useReceipts'

const { totalAmount, totalReceipts, mostPopularCategory, fetchReceipts } = useReceipts();

  configure({
    validateOnBlur: true,
    validateOnChange: true,
    validateOnInput: false,
    validateOnModelUpdate: false,
  });

  interface FormValues {
    username: string;
    email: string;
    password: string;
  }

  const form = ref<FormActions<FormValues>>();

  const currentSchema = {
    currentPassword: string()
      .required("Текущий пароль обязателен")
      .min(5, "Пароль должен быть длиннее 5 символов"),
    newPassword: string()
      .required("Новый пароль обязателен")
      .min(5, "Пароль должен быть длиннее 5 символов"),
    confirmPassword: string()
      .required("Подтверждение пароля обязательно")
      .min(5, "Пароль должен быть длиннее 5 символов")
  };

interface CropperInstance {
  getResult: () => {
    canvas: HTMLCanvasElement;
  };
  reset: () => void;
}

const { getUserInfo, uploadAvatar, deleteAvatar, getAvatar, changePassword, avatarUrl, username, email, registrationTime } = useUsers()
const { errorMessage, handleApiError } = useErrorHandler();
const fileInput = ref<HTMLInputElement | null>(null)
const uploadError = ref<string | null>(null)
const showCropper = ref(false)
const imageSrc = ref('')
const cropperRef = ref<CropperInstance | null>(null)
const showDeleteConfirm = ref(false)
const showPasswordChange = ref(false)

const triggerFileInput = () => {
  uploadError.value = null
  fileInput.value?.click()
}

const passwordForm = ref({
  currentPassword: '',
  newPassword: '',
  confirmPassword: ''
});
const passwordError = ref<string | null>(null);
const passwordSuccess = ref<string | null>(null);
const isChangingPassword = ref(false);

const handlePasswordChange = async () => {
  passwordError.value = null;
  passwordSuccess.value = null;
  errorMessage.value = null;

  if (passwordForm.value.newPassword !== passwordForm.value.confirmPassword) {
    passwordError.value = 'Новый пароль и подтверждение не совпадают';
    return;
  }

  try {
    isChangingPassword.value = true;
    await changePassword(
      passwordForm.value.currentPassword,
      passwordForm.value.newPassword
    );

    passwordSuccess.value = 'Пароль успешно изменён';
    passwordForm.value = {
      currentPassword: '',
      newPassword: '',
      confirmPassword: ''
    };
  } catch (error) {
    handleApiError(error);
    passwordError.value = errorMessage.value;
  } finally {
    isChangingPassword.value = false;
  }
};

const handleFileSelect = (e: Event) => {
  const file = (e.target as HTMLInputElement).files?.[0]
  if (!file) return

  if (!file.type.startsWith('image/')) {
    uploadError.value = 'Пожалуйста, выберите изображение'
    return
  }

  const reader = new FileReader()
  reader.onload = (event) => {
    imageSrc.value = event.target?.result as string
    showCropper.value = true
  }
  reader.readAsDataURL(file)
}

const cropImage = async () => {
  if (!cropperRef.value) return

  const result = cropperRef.value.getResult()
  const canvas = result.canvas

  if (!canvas) {
    uploadError.value = 'Не удалось обрезать изображение'
    return
  }

  canvas.toBlob(async (blob) => {
    if (!blob) {
      uploadError.value = 'Не удалось обрезать изображение'
      return
    }

    try {
      const croppedFile = new File([blob], 'avatar.jpg', { type: 'image/jpeg' })
      await uploadAvatar(croppedFile)
      showCropper.value = false
      imageSrc.value = ''
    } catch (error) {
      uploadError.value = error.response?.data?.message || 'Ошибка при загрузке аватара'
      console.error('Ошибка при загрузке аватара:', error)
    }
  }, 'image/jpeg', 0.9)
}

const cancelCrop = () => {
  if (cropperRef.value) {
    cropperRef.value.reset()
  }
  showCropper.value = false
  imageSrc.value = ''
  if (fileInput.value) fileInput.value.value = ''
}

const confirmDeleteAvatar = async () => {
  try {
    uploadError.value = null
    await deleteAvatar()
    showDeleteConfirm.value = false
  } catch (error) {
    uploadError.value = error.response?.data?.message || 'Ошибка при удалении аватара'
    console.error('Ошибка при удалении аватара:', error)
    showDeleteConfirm.value = false
  }
}

onMounted(async () => {
  await getUserInfo()
  await getAvatar()
  await fetchReceipts()
})
</script>

<style lang="scss" scoped>
.profile {
  padding: 0 20px;

  &__container {
    width: 100%;
    max-width: 1280px;
    margin: 0 auto;
    box-sizing: border-box;
  }

  &__header {
    display: flex;
    align-items: center;
    margin-bottom: 32px;
    background-color: var(--vt-c-dark-blue-gray);
    border-radius: 16px;
    padding: 24px;
    box-shadow: 0 4px 6px -1px rgba(0, 0, 0, 0.1);

    @media (max-width: 768px) {
      flex-direction: column;
      text-align: center;
    }
  }

  &__avatar {
    margin-right: 24px;
    position: relative;

    @media (max-width: 768px) {
      margin-right: 0;
      margin-bottom: 16px;
      display: flex;
      flex-direction: column;
      justify-content: center;
      align-items: center;
    }
  }

  &__avatar-inner {
    width: 100px;
    height: 100px;
    border-radius: 50%;
    display: flex;
    align-items: center;
    justify-content: center;
    position: relative;
    cursor: pointer;
    overflow: hidden;

    @media (hover: hover) and (pointer: fine) {
      &:hover {
        opacity: 1;
      }
    }

    @media (max-width: 768px) {
      width: 80px;
      height: 80px;
    }
  }

  &__avatar-image {
    width: 100%;
    height: 100%;
    object-fit: cover;
  }

  &__avatar-input {
    display: none;
  }

  &__avatar-overlay {
    position: absolute;
    top: 0;
    left: 0;
    right: 0;
    bottom: 0;
    background: rgba(0, 0, 0, 0.5);
    display: flex;
    align-items: center;
    justify-content: center;
    opacity: 0;
    transition: opacity 0.3s ease;
    color: white;

    @media (hover: hover) and (pointer: fine) {
      &:hover {
        opacity: 1;
      }
    }
  }

  &__avatar-actions {
    display: flex;
    justify-content: center;
    margin-top: 12px;
  }

  &__avatar-delete {
    display: flex;
    align-items: center;
    justify-content: center;
    gap: 6px;
    padding: 6px 12px;
    background: none;
    color: var(--vt-c-light-red);
    border: 2px solid var(--vt-c-light-red);
    border-radius: 8px;
    cursor: pointer;
    font-size: 14px;
    font-weight: 500;
    transition: all 0.2s ease;

    @media (hover: hover) and (pointer: fine) {
      &:hover {
        background: var(--vt-c-light-red);
        color: var(--vt-c-white);
      }
    }

    svg {
      flex-shrink: 0;
    }

    @media (max-width: 768px) {
      padding: 8px 16px;
    }
  }

  &__password-error {
    color: var(--vt-c-light-red);
    font-size: 14px;
  }

  &__password-form {
    margin-top: 16px;
    display: grid;
    gap: 16px;
    max-width: 400px;
    width: 100%;

    @media (max-width: 768px) {
      max-width: 100%;
      gap: 12px;
    }
  }

  &__form-group {
    display: grid;
    gap: 6px;
  }

  &__form-label {
    font-size: 13px;
    color: var(--vt-c-light-gray);
    font-weight: 500;
    padding-left: 4px;
  }

  &__form-input {
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

  &__form-message {
    font-size: 13px;
    padding: 8px 12px;
    border-radius: 6px;
    margin: 4px 0;

    &--error {
      color: var(--vt-c-light-red);
      background-color: rgba(220, 53, 69, 0.08);
    }

    &--success {
      color: var(--primary-green);
      background-color: rgba(137, 225, 89, 0.08);
    }
  }

  &__form-submit {
    width: 100%;
    padding: 10px 16px;
    background-color: var(--primary-green);
    color: var(--vt-c-dark-blue-gray);
    border: 2px solid var(--primary-green);
    border-radius: 6px;
    font-size: 16px;
    font-weight: 600;
    cursor: pointer;
    transition: all 0.2s ease;

    @media (hover: hover) and (pointer: fine) {
      &:hover {
        background: none;
        color: var(--primary-green);
      }
    }
  }

  &__change-password {
    width: 100%;
    max-width: 300px;
    padding: 10px 16px;
    background-color: var(--primary-green);
    color: var(--vt-c-dark-blue-gray);
    border: 2px solid var(--primary-green);
    border-radius: 6px;
    font-size: 16px;
    font-weight: 600;
    cursor: pointer;
    transition: all 0.2s ease;

    @media (max-width: 768px) {
      max-width: 100%;
    }

    @media (hover: hover) and (pointer: fine) {
      &:hover {
        background: none;
        color: var(--primary-green);
      }
    }
  }

  &__info {
    flex: 1;
  }

  &__name {
    font-size: 24px;
    font-weight: 600;
    color: var(--vt-c-white);
    margin: 0 0 8px 0;
  }

  &__email {
    font-size: 16px;
    color: var(--vt-c-light-gray);
    margin: 0;
  }

  &__stats {
    display: grid;
    grid-template-columns: repeat(3, 1fr);
    gap: 20px;
    margin-bottom: 32px;

    @media (max-width: 768px) {
      grid-template-columns: 1fr;
    }
  }

  &__sections {
    display: grid;
    gap: 32px;
  }

  &__section {
    background-color: var(--vt-c-dark-blue-gray);
    border-radius: 16px;
    padding: 24px;
    box-shadow: 0 4px 6px -1px rgba(0, 0, 0, 0.1);
  }

  &__section-title {
    font-size: 18px;
    font-weight: 600;
    color: var(--vt-c-white);
    margin: 0 0 20px 0;
  }

  &__info-grid {
    display: grid;
    grid-template-columns: repeat(2, 1fr);
    gap: 20px;

    @media (max-width: 768px) {
      grid-template-columns: 1fr;
    }
  }

  &__info-item {
    label {
      display: block;
      font-size: 14px;
      color: var(--vt-c-light-gray);
      margin-bottom: 8px;
    }

    p {
      font-size: 16px;
      color: var(--vt-c-white);
      margin: 0;
    }
  }

  &__avatar-error {
    color: var(--vt-c-red);
    font-size: 12px;
    margin-top: 8px;
    text-align: center;
  }

  &__modal {
    position: fixed;
    top: 0;
    left: 0;
    right: 0;
    bottom: 0;
    background: rgba(0, 0, 0, 0.8);
    display: flex;
    align-items: center;
    justify-content: center;
    z-index: 1000;
    padding: 20px;
  }

  &__modal-container {
    width: 100%;
    max-width: 500px;
    background: var(--vt-c-dark-blue-gray);
    border-radius: 16px;
    box-shadow: 0 4px 20px rgba(0, 0, 0, 0.3);
    overflow: hidden;

    &--small {
      max-width: 400px;
    }
  }

  &__modal-header {
    display: flex;
    align-items: center;
    justify-content: space-between;
    padding: 16px 20px;
    border-bottom: 1px solid rgba(255, 255, 255, 0.1);
  }

  &__modal-title {
    font-size: 18px;
    font-weight: 600;
    color: var(--vt-c-white);
    margin: 0;
  }

  &__modal-close {
    background: transparent;
    border: none;
    color: var(--vt-c-light-gray);
    cursor: pointer;
    padding: 4px;
    display: flex;
    align-items: center;
    justify-content: center;
    transition: color 0.2s ease;

    &:hover {
      color: var(--vt-c-white);
    }
  }

  &__modal-content {
    padding: 24px 20px;
    display: flex;
    flex-direction: column;
    align-items: center;
    text-align: center;
  }

  &__modal-icon {
    margin-bottom: 16px;

    &--warning {
      color: var(--vt-c-red);
    }
  }

  &__modal-message {
    font-size: 16px;
    color: var(--vt-c-white);
    margin: 0;
  }

  &__modal-actions {
    display: flex;
    justify-content: flex-end;
    gap: 12px;
    padding: 16px 20px;
    border-top: 1px solid rgba(255, 255, 255, 0.1);
  }

  &__modal-button {
    padding: 10px 16px;
    border-radius: 8px;
    font-weight: 500;
    font-size: 14px;
    cursor: pointer;
    transition: all 0.2s ease;
    border: none;

    &--cancel {
      background: none;
      border: 2px solid var(--vt-c-white);
      color: var(--vt-c-white);
      font-size: 16px;
      font-weight: 600;

      &:hover {
        background: var(--vt-c-white);
        color: var(--vt-c-dark-blue-gray);
      }
    }

    &--confirm {
      border: 2px solid var(--primary-green);
      background: var(--primary-green);
      color: var(--vt-c-dark-blue-gray);
      font-size: 16px;
      font-weight: 600;

      &:hover {
        background: none;
        color: var(--primary-green);
      }
    }
  }

  .cropper {
    width: 100%;
    height: 300px;
    background: var(--vt-c-dark-blue-gray);

    @media (max-width: 768px) {
      height: 250px;
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
      z-index: 10000;
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
      width: 100%;
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
