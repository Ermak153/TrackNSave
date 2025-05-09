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
          <button v-if="avatarUrl" class="profile__avatar-delete" @click="handleDeleteAvatar">
            Удалить аватар
          </button>
          <p v-if="uploadError" class="profile__avatar-error">{{ uploadError }}</p>
        </div>
        <div class="profile__info">
          <h1 class="profile__name">123123</h1>
          <p class="profile__email">123123@yandex.ru</p>
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
            <h3 class="info-card__value">127</h3>
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
            <h3 class="info-card__value">45,678 ₽</h3>
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
            <h3 class="info-card__value">Продукты питания</h3>
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
              <p class="profile__info-value">123123</p>
            </div>
            <div class="profile__info-item">
              <label class="profile__info-label">В приложении</label>
              <p class="profile__info-value">1 год 3 мес</p>
            </div>
            <div class="profile__info-item">
              <label class="profile__info-label">Email</label>
              <p class="profile__info-value">123123@yandex.ru</p>
            </div>
          </div>
        </div>

        <div class="profile__section">
          <h2 class="profile__section-title">Настройки</h2>
          <div class="profile__settings">
            <div class="profile__setting-item">
              <div class="profile__setting-info">
                <svg class="profile__setting-icon" xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
                  <path d="M18 8A6 6 0 0 0 6 8c0 7-3 9-3 9h18s-3-2-3-9"></path>
                  <path d="M13.73 21a2 2 0 0 1-3.46 0"></path>
                </svg>
                <span>Уведомления</span>
              </div>
              <label class="switch">
                <input type="checkbox" checked>
                <span class="slider round"></span>
              </label>
            </div>
            <div class="profile__setting-item">
              <div class="profile__setting-info">
                <svg class="profile__setting-icon" xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
                  <path d="M21 12.79A9 9 0 1 1 11.21 3 7 7 0 0 0 21 12.79z"></path>
                </svg>
                <span>Темная тема</span>
              </div>
              <label class="switch">
                <input type="checkbox">
                <span class="slider round"></span>
              </label>
            </div>
            <div class="profile__setting-item">
              <div class="profile__setting-info">
                <svg class="profile__setting-icon" xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
                  <circle cx="12" cy="12" r="10"></circle>
                  <line x1="2" y1="12" x2="22" y2="12"></line>
                  <path d="M12 2a15.3 15.3 0 0 1 4 10 15.3 15.3 0 0 1-4 10 15.3 15.3 0 0 1-4-10 15.3 15.3 0 0 1 4-10z"></path>
                </svg>
                <span>Язык</span>
              </div>
              <span class="profile__setting-value">Русский</span>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Модальное окно для обрезки -->
    <div v-if="showCropper" class="profile__cropper-modal">
      <div class="profile__cropper-container">
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
        <div class="profile__cropper-actions">
          <button @click="cancelCrop" class="profile__cropper-button profile__cropper-button--cancel">
            Отмена
          </button>
          <button @click="cropImage" class="profile__cropper-button profile__cropper-button--confirm">
            Применить
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { Cropper } from 'vue-advanced-cropper'
import 'vue-advanced-cropper/dist/style.css'
import { useUsers } from '@/composables/useUsers'
import CustomBackgroundWrapper from '@/components/CustomBackgroundWrapper.vue'
import CircleStencil from '@/components/CircleStencil.vue'

const defaultAvatar = "https://cube.elemecdn.com/3/7c/3ea6beec64369c2642b92c6726f1epng.png";

interface CropperInstance {
  getResult: () => {
    canvas: HTMLCanvasElement;
  };
  reset: () => void;
}

const { getUserInfo, uploadAvatar, deleteAvatar, avatarUrl, getAvatar } = useUsers()
const fileInput = ref<HTMLInputElement | null>(null)
const uploadError = ref<string | null>(null)
const showCropper = ref(false)
const imageSrc = ref('')
const cropperRef = ref<CropperInstance | null>(null)

const triggerFileInput = () => {
  uploadError.value = null
  fileInput.value?.click()
}

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

const handleDeleteAvatar = async () => {
  try {
    uploadError.value = null
    await deleteAvatar()
  } catch (error) {
    uploadError.value = error.response?.data?.message || 'Ошибка при удалении аватара'
    console.error('Ошибка при удалении аватара:', error)
  }
}

onMounted(async () => {
  await getUserInfo()
  await getAvatar()
})
</script>

<style lang="scss" scoped>
.profile {
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

    &:hover .profile__avatar-overlay {
      opacity: 1;
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

  &__avatar-text {
    font-size: 36px;
    font-weight: bold;
    color: white;

    @media (max-width: 768px) {
      font-size: 28px;
    }
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
  }

  &__avatar-delete {
    margin-top: 8px;
    padding: 4px 8px;
    background: var(--vt-c-red);
    color: white;
    border: none;
    border-radius: 4px;
    cursor: pointer;
    font-size: 12px;
    transition: background-color 0.3s ease;

    &:hover {
      background: var(--vt-c-red);
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

  &__settings {
    display: flex;
    flex-direction: column;
    gap: 16px;
  }

  &__setting-item {
    display: flex;
    align-items: center;
    justify-content: space-between;
    padding: 12px 0;
  }

  &__setting-info {
    display: flex;
    align-items: center;
    gap: 12px;
    color: var(--vt-c-white);
  }

  &__setting-icon {
    color: var(--primary-green);
    width: 20px;
    height: 20px;
  }

  &__setting-value {
    color: var(--vt-c-light-gray);
  }

  &__avatar-error {
    color: var(--vt-c-red);
    font-size: 12px;
    margin-top: 8px;
    text-align: center;
  }

  &__cropper-modal {
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
  }

  &__cropper-container {
    width: 90%;
    max-width: 500px;
    background: var(--vt-c-dark-blue-gray);
    border-radius: 16px;
    padding: 20px;
    box-shadow: 0 4px 20px rgba(0, 0, 0, 0.3);

    .cropper {
      width: 100%;
      height: 300px;
      margin-bottom: 20px;
      background: var(--vt-c-dark-blue-gray);

      @media (max-width: 768px) {
        height: 250px;
      }
    }
  }

  &__cropper-actions {
    display: flex;
    justify-content: flex-end;
    gap: 12px;
    margin-top: 20px;
  }

  &__cropper-button {
    padding: 8px 16px;
    border-radius: 8px;
    font-weight: 500;
    cursor: pointer;
    transition: all 0.2s ease;
    border: none;

    &--cancel {
      background: var(--vt-c-blue-gray);
      color: var(--vt-c-white);

      &:hover {
        background: var(--vt-c-blue-gray);
      }
    }

    &--confirm {
      background: var(--primary-green);
      color: var(--vt-c-black);

      &:hover {
        background: var(--primary-green);
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

.switch {
  position: relative;
  display: inline-block;
  width: 50px;
  height: 24px;

  input {
    opacity: 0;
    width: 0;
    height: 0;
  }

  .slider {
    position: absolute;
    cursor: pointer;
    top: 0;
    left: 0;
    right: 0;
    bottom: 0;
    background-color: var(--vt-c-light-background);
    transition: .4s;
    border: 1px solid var(--vt-c-blue-gray);

    &:before {
      position: absolute;
      content: "";
      height: 16px;
      width: 16px;
      left: 4px;
      bottom: 3px;
      background-color: var(--vt-c-blue-gray);
      transition: .4s;
    }

    &.round {
      border-radius: 24px;

      &:before {
        border-radius: 50%;
      }
    }
  }

  input:checked + .slider {
    background-color: var(--primary-green);
    border-color: var(--primary-green);

    &:before {
      transform: translateX(26px);
      background-color: var(--vt-c-white);
    }
  }
}
</style>
