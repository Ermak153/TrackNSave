<template>
  <div class="upload">
    <div v-if="scanSuccess" class="upload__success-container">
      <div class="upload__success-screen">
        <div class="upload__success-icon">
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
        <p class="upload__success-message">
          {{
            successCount > 1
              ? `${successCount} чеков успешно добавлено`
              : "Чек успешно добавлен"
          }}
        </p>
      </div>

      <div class="upload__success-buttons">
        <button
          class="upload__button upload__button--cancel"
          @click="$emit('close')"
        >
          Закрыть
        </button>
        <button
          class="upload__button upload__button--submit"
          @click="resetForm"
        >
          Добавить ещё
        </button>
      </div>
    </div>

    <div v-if="!scanSuccess">
      <div
        class="upload__area"
        :class="{ 'upload__area--dragover': dragOver }"
        @dragover.prevent="dragOver = true"
        @dragleave.prevent="dragOver = false"
        @drop="onDrop"
        @click="openFileInput"
      >
        <img :src="uploadIcon" class="upload__area--icon" />
        <span
          ><em class="upload__area--highlight">Нажмите,</em> чтобы загрузить
          файл, либо перетащите сюда</span
        >
        <span>PNG, JPG, GIF или TIFF до 10 МБ</span>
        <input
          ref="fileInput"
          type="file"
          multiple
          hidden
          @change="handleFileSelected"
        />
      </div>

      <div class="upload__line" v-if="fileItems.length"></div>

      <ul class="upload__list">
        <li
          v-for="(item, index) in fileItems"
          :key="index"
          class="upload__item"
          :class="{
            'upload__item--error':
              item.error ||
              item.qrResult === 'QR-код не найден' ||
              item.isDuplicate,
          }"
        >
          <div class="upload__item-wrapper">
            <div class="upload__item-icon">
              <img :src="fileIcon" />
            </div>
            <div class="upload__item-content">
              <div class="upload__item-header">
                <div class="upload__item-info">
                  <span class="upload__item-name">{{ item.name }}</span>
                  <span class="upload__item-size">{{
                    formatFileSize(item.size)
                  }}</span>
                </div>
                <button class="upload__item-remove" @click="removeFile(index)">
                  &times;
                </button>
              </div>

              <div class="upload__item-progress">
                <div class="upload__item-progress-wrapper">
                  <div
                    class="upload__item-progress-bar"
                    :style="{ width: item.progress + '%' }"
                  ></div>
                </div>
                <span class="upload__progress-text">{{ item.progress }}%</span>
              </div>

              <div
                v-if="item.qrResult === 'QR-код не найден'"
                class="upload__qr-result upload__qr-result--error"
              >
                QR-код не найден
              </div>

              <div
                v-if="item.isDuplicate"
                class="upload__qr-result upload__qr-result--error"
              >
                Дубликат QR-кода
              </div>

              <div
                v-if="item.apiError"
                class="upload__qr-result upload__qr-result--error"
              >
                {{ item.apiError }}
              </div>
            </div>
          </div>
        </li>
      </ul>

      <div class="upload__buttons-group">
        <button
          class="upload__button upload__button--cancel"
          @click="$emit('close')"
        >
          Отмена
        </button>
        <button
          type="submit"
          class="upload__button upload__button--submit"
          :disabled="!canSubmit || isLoading"
          @click="submitQrCodes"
        >
          {{ isLoading ? "Отправка..." : "Добавить" }}
        </button>
      </div>
    </div>
  </div>
</template>

<script lang="ts" setup>
  import { ref, computed } from "vue";
  import uploadIcon from "@/assets/icons/upload.svg";
  import fileIcon from "@/assets/icons/file.svg";
  import jsQR from "jsqr";
  import api from "@/api/axios";
  import { useReceipts } from "@/composables/useReceipts";
  import { useErrorHandler } from "@/composables/useErrorHandler";

  const { fetchReceipts } = useReceipts();
  const { errorMessage, handleApiError } = useErrorHandler();

  interface FileItem {
    file: File;
    name: string;
    size: number;
    completed: boolean;
    error: boolean;
    progress: number;
    qrResult?: string;
    isDuplicate?: boolean;
    apiError?: string;
  }

  const fileItems = ref<FileItem[]>([]);
  const dragOver = ref(false);
  const fileInput = ref<HTMLInputElement | null>(null);
  const isLoading = ref(false);
  const scanSuccess = ref(false);
  const successCount = ref(0);

  const canSubmit = computed(() => {
    if (fileItems.value.length === 0) return false;

    const hasValidQr = fileItems.value.some(
      (item) =>
        item.completed &&
        item.qrResult &&
        item.qrResult !== "QR-код не найден" &&
        !item.isDuplicate
    );

    const allProcessed = fileItems.value.every(
      (item) => item.completed || item.error
    );

    return hasValidQr && allProcessed;
  });

  const onDrop = (event: DragEvent) => {
    event.preventDefault();
    dragOver.value = false;
    if (event.dataTransfer?.files) {
      for (let i = 0; i < event.dataTransfer.files.length; i++) {
        const file = event.dataTransfer.files[i];
        if (validateFile(file)) {
          addFile(file);
        }
      }
    }
  };

  const formatFileSize = (size: number) => {
    if (size < 1024) return `${size} Б`;
    if (size < 1024 * 1024) return `${(size / 1024).toFixed(1)} КБ`;
    return `${(size / (1024 * 1024)).toFixed(1)} МБ`;
  };

  const openFileInput = () => {
    fileInput.value?.click();
  };

  const validateFile = (file: File) => {
    const allowedTypes = [
      "image/png",
      "image/jpg",
      "image/jpeg",
      "image/gif",
      "image/tiff",
    ];
    const maxSize = 10 * 1024 * 1024;
    if (!allowedTypes.includes(file.type)) {
      alert("Недопустимый формат файла!");
      return false;
    }
    if (file.size > maxSize) {
      alert("Файл слишком большой!");
      return false;
    }
    return true;
  };

  const addFile = (file: File) => {
    const fileItem: FileItem = {
      file,
      name: file.name,
      size: file.size,
      completed: false,
      error: false,
      progress: 0,
    };
    fileItems.value.push(fileItem);
    readFile(fileItem);
  };

  const readFile = (fileItem: FileItem) => {
    const reader = new FileReader();

    reader.onload = () => {
      const index = fileItems.value.findIndex((f) => f.file === fileItem.file);
      if (index !== -1) {
        fileItems.value[index].progress = 100;
        fileItems.value[index].completed = true;
        const imageData = reader.result as string;

        const image = new Image();
        image.src = imageData;

        image.onload = () => {
          const canvas = document.createElement("canvas");
          const context = canvas.getContext("2d");
          if (context) {
            const maxWidth = 800;
            const maxHeight = 800;
            const scale = Math.min(
              maxWidth / image.width,
              maxHeight / image.height,
              1
            );
            canvas.width = image.width * scale;
            canvas.height = image.height * scale;
            context.drawImage(image, 0, 0, canvas.width, canvas.height);

            let imageData = context.getImageData(
              0,
              0,
              canvas.width,
              canvas.height
            );
            let code = jsQR(imageData.data, canvas.width, canvas.height, {
              inversionAttempts: "attemptBoth",
            });

            if (!code) {
              imageData = context.getImageData(0, 0, canvas.width, canvas.height);
              const pixels = imageData.data;

              for (let i = 0; i < pixels.length; i += 4) {
                for (let j = 0; j < 3; j++) {
                  pixels[i + j] =
                    pixels[i + j] < 128
                      ? pixels[i + j] * 0.8
                      : Math.min(255, pixels[i + j] * 1.2);
                }
              }

              context.putImageData(imageData, 0, 0);
              code = jsQR(imageData.data, canvas.width, canvas.height, {
                inversionAttempts: "attemptBoth",
              });
            }

            if (code) {
              fileItems.value[index].qrResult = code.data;
              checkForDuplicates();
            } else {
              fileItems.value[index].qrResult = "QR-код не найден";
              fileItems.value[index].error = true;
            }
          }
        };
      }
    };

    reader.readAsDataURL(fileItem.file);
  };

  const handleFileSelected = (event: Event) => {
    const files = (event.target as HTMLInputElement).files;
    if (files && files.length > 0) {
      for (let i = 0; i < files.length; i++) {
        const file = files[i];
        if (validateFile(file)) {
          addFile(file);
        }
      }
    }
  };

  const removeFile = (index: number) => {
    if (index < fileItems.value.length) {
      fileItems.value.splice(index, 1);
      checkForDuplicates();
    }
  };

  const checkForDuplicates = () => {
    const qrCodes = new Map();

    fileItems.value.forEach((item) => {
      item.isDuplicate = false;
    });

    fileItems.value.forEach((item) => {
      if (item.qrResult && item.qrResult !== "QR-код не найден") {
        if (qrCodes.has(item.qrResult)) {
          item.isDuplicate = true;
        } else {
          qrCodes.set(item.qrResult, true);
        }
      }
    });
  };

  const resetForm = () => {
    scanSuccess.value = false;
    successCount.value = 0;
    fileItems.value = [];
    errorMessage.value = null;
  };

  const submitQrCodes = async () => {
    try {
      isLoading.value = true;

      const validItems = fileItems.value.filter(
        (item) =>
          item.completed &&
          item.qrResult &&
          item.qrResult !== "QR-код не найден" &&
          !item.isDuplicate &&
          !item.error
      );

      if (validItems.length === 0) {
        isLoading.value = false;
        return;
      }

      successCount.value = 0;

      for (const item of validItems) {
        try {
          await api.post("/receipt/add", { ReceiptRaw: item.qrResult });
          successCount.value++;
        } catch (error) {
          item.error = true;
          handleApiError(error);
          item.apiError = errorMessage.value || "Ошибка при добавлении чека";
          errorMessage.value = null;
        }
      }

      await fetchReceipts();

      if (successCount.value > 0) {
        scanSuccess.value = true;
      }
    } catch (error) {
      console.error("Общая ошибка:", error);
    } finally {
      isLoading.value = false;
    }
  };
</script>

<style lang="scss" scoped>
  .upload {
    width: 100%;
    margin-top: 20px;

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

    &__qr-result--error {
      color: var(--vt-c-light-red);
    }

    &__line {
      width: 100%;
      height: 1px;
      background-color: var(--vt-c-blue-gray);
      margin: 15px 0;
    }

    &__list {
      list-style: none;
      padding: 0;
      margin: 0;
      max-height: 250px;
      overflow-y: auto;
      overflow-x: hidden;
      max-width: 100%;
      box-sizing: border-box;
      gap: 15px;
      display: flex;
      flex-direction: column;
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

    &__area {
      text-align: center;
      box-sizing: border-box;
      display: flex;
      justify-content: center;
      align-items: center;
      flex-direction: column;
      font-size: 14px;
      padding: 10px;
      border-radius: 10px;
      border: 2px solid var(--primary-green);
      background-color: rgba(137, 225, 89, 0.1);
      transition: 0.2s;
      cursor: pointer;

      &--icon {
        height: 40px;
        width: 40px;
      }

      &--highlight {
        color: var(--primary-green);
        font-style: normal;
        transition: 0.2s;
      }

      &--dragover {
        background-color: rgba(6, 157, 61, 0.1);
        border: 2px solid var(--vt-c-dark-green);
        transition: 0.2s;

        .upload__area--highlight {
          color: var(--vt-c-dark-green);
          transition: 0.2s;
        }
      }

      @media (hover: hover) and (pointer: fine) {
        &:hover {
          background-color: rgba(6, 157, 61, 0.1);
          border: 2px solid var(--vt-c-dark-green);
          transition: 0.2s;
        }

        &:hover &--highlight {
          color: var(--vt-c-dark-green);
          transition: 0.2s;
        }
      }
    }

    &__item {
      width: 100%;
      box-sizing: border-box;
      border-radius: 10px;
      border: 2px solid var(--primary-green);
      padding: 10px;

      &--error {
        border-color: var(--vt-c-light-red);
      }

      &-icon {
        margin-right: 20px;
      }

      &-name {
        font-size: 16px;
        color: var(--vt-c-white);
        max-width: 200px;
        white-space: nowrap;
        overflow: hidden;
        text-overflow: ellipsis;
      }

      &-size {
        font-size: 14px;
        color: var(--vt-c-light-gray);
      }

      &-info {
        display: flex;
        flex-direction: column;
        overflow: hidden;
      }

      &-progress {
        display: flex;
        justify-content: center;
        align-items: center;

        &-wrapper {
          width: 100%;
          border-radius: 5px;
          border: 1px solid var(--primary-green);
          height: 6px;
          margin-right: 10px;
        }

        &-bar {
          height: 100%;
          background: var(--primary-green);
          border-radius: 5px;
        }
      }

      &-remove {
        background: none;
        border: none;
        font-size: 24px;
        color: var(--vt-c-white);
        transition: 0.2s;
        cursor: pointer;

        @media (hover: hover) and (pointer: fine) {
          &:hover {
            color: var(--vt-c-light-red);
          }
        }
      }

      &-header {
        display: flex;
        justify-content: space-between;
        align-items: start;
        max-width: 100%;
        overflow: hidden;
      }

      &-content {
        display: flex;
        flex-direction: column;
        width: 100%;
      }

      &-wrapper {
        display: flex;
        width: 100%;
      }
    }
  }
</style>
