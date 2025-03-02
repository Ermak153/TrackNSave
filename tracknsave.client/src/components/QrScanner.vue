<template>
  <div class="scanner__wrapper">
    <div class="scanner__container">
      <video
        v-if="!scanSuccess"
        ref="videoRef"
        class="scanner__video"
        autoplay
        muted
        playsinline
      ></video>

      <div v-if="!scanSuccess" class="scanner__overlay">
        <div class="scanner__region">
          <div class="scanner__line"></div>
        </div>
      </div>

      <div v-if="scanSuccess" class="scanner__success-screen">
        <div class="scanner__success-icon">
          <svg width="80" height="80" viewBox="0 0 80 80" fill="none" xmlns="http://www.w3.org/2000/svg">
            <circle cx="40" cy="40" r="38" stroke="var(--primary-green)" stroke-width="4"/>
            <path d="M25 40L35 50L55 30" stroke="var(--primary-green)" stroke-width="4" stroke-linecap="round" stroke-linejoin="round"/>
          </svg>
        </div>
        <p class="scanner__success-message">Чек успешно добавлен</p>
      </div>
    </div>

    <div>
      <p v-if="errorMessage" class="scanner__error-message">
        {{ errorMessage }}
      </p>

      <div v-if="!scanSuccess">
        <button
          class="scanner__flash-button"
          :class="{ 'scanner__flash-button--on': isFlashOn }"
          @click="toggleFlash"
          :disabled="!hasFlash"
        >
          {{ isFlashOn ? "Выключить вспышку" : "Включить вспышку" }}
        </button>
      </div>
      <div v-else class="scanner__success-buttons">
        <button
          class="scanner__button scanner__button--close"
          @click="closeScanner"
        >
          Закрыть
        </button>
        <button
          class="scanner__button scanner__button--add-more"
          @click="resetAndScanAgain"
        >
          Добавить ещё
        </button>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
  import { ref, onMounted, onUnmounted } from "vue";
  import api from "@/api/axios";
  import QrScanner from "qr-scanner";
  import { useReceipts } from "@/composables/useReceipts";
  import { useErrorHandler } from "@/composables/useErrorHandler";

  const { errorMessage, handleApiError } = useErrorHandler();
  const videoRef = ref<HTMLVideoElement | null>(null);
  const scanResult = ref<string | null>(null);
  const isFlashOn = ref<boolean>(false);
  const hasFlash = ref<boolean>(false);
  const scanSuccess = ref<boolean>(false);

  const { fetchReceipts } = useReceipts();
  const emit = defineEmits(["close"]);

  let qrScanner: QrScanner | null = null;
  let lastScanTime = 0;

  const stopScanner = () => {
    qrScanner?.stop?.();
    qrScanner?.destroy?.();
  };

  const closeScanner = () => {
    emit("close");
  };

  const resetAndScanAgain = () => {
    scanSuccess.value = false;
    errorMessage.value = null;
    lastScannedCode.value = null;
    scanResult.value = null;

    setTimeout(async () => {
      await initScanner();
    }, 100);
  };

  const updateBorderColors = () => {
    const color = getComputedStyle(document.documentElement)
      .getPropertyValue("--primary-green")
      .trim();
    const elements = [
      document.querySelector<SVGElement>(".code-outline-highlight polygon"),
      document.querySelector<SVGElement>(".scan-region-highlight-svg path"),
      document.querySelector<HTMLDivElement>(".scan-region-highlight"),
    ];

    elements.forEach((element) => {
      if (element) {
        element.style.stroke = color;
        if (element instanceof HTMLDivElement) {
          element.style.zIndex = "1";
        }
      }
    });
  };

  const toggleFlash = async () => {
    if (!qrScanner) return;

    try {
      if (isFlashOn.value) {
        await qrScanner.turnFlashOff();
      } else {
        await qrScanner.turnFlashOn();
      }
      isFlashOn.value = !isFlashOn.value;
    } catch {
      errorMessage.value = "Не удалось изменить состояние вспышки";
    }
  };

  const lastScannedCode = ref<string | null>(null);

  const scanReceipt = async (receiptRaw: string) => {
    if (lastScannedCode.value === receiptRaw) return;
    lastScannedCode.value = receiptRaw;

    try {
      const response = await api.post("/receipt/add", { ReceiptRaw: receiptRaw });
      scanResult.value = response.data;
      await fetchReceipts();
      stopScanner();
      scanSuccess.value = true;
      errorMessage.value = null;
    } catch (error) {
      handleApiError(error);
    }
  };

  const initScanner = async () => {
    if (!videoRef.value) return;

    qrScanner = new QrScanner(
      videoRef.value,
      (result: QrScanner.ScanResult) => {
        const now = Date.now();
        if (now - lastScanTime >= 1000) {
          lastScanTime = now;
          scanReceipt(result.data);
        }
      },
      {
        returnDetailedScanResult: true,
        highlightScanRegion: true,
        highlightCodeOutline: true,
      }
    );

    try {
      await qrScanner.start();
      hasFlash.value = await qrScanner.hasFlash();
    } catch (error) {
        const errorText = error instanceof Error ? error.message : String(error);
        if (errorText.includes("Camera not found")) {
        errorMessage.value = "Камера не найдена. Проверьте её подключение и доступ.";
      } else {
        errorMessage.value = errorText;
      }
    }

    updateBorderColors();
  }

  onMounted(async () => {
    await initScanner();
  });

  onUnmounted(() => {
    stopScanner();
  });
</script>

<style lang="scss" scoped>
  .scanner {
    &__wrapper {
      flex-shrink: 0;
      margin-top: 15px;
    }

    &__container {
      position: relative;
      width: 100%;
      padding-top: 100%;
      overflow: hidden;
      border-radius: 8px;
      margin-bottom: 15px;
    }

    &__video {
      position: absolute;
      top: 0;
      left: 0;
      width: 100%;
      height: 100%;
      object-fit: cover;
    }

    &__overlay {
      position: absolute;
      display: flex;
      align-items: center;
      justify-content: center;
      top: 0;
      left: 0;
      right: 0;
      bottom: 0;
      background: rgba(0, 0, 0, 0.3);
    }

    &__region {
      width: 75%;
      height: 75%;
      position: relative;
      border: 2px solid rgba(255, 255, 255, 0.8);
    }

    &__line {
      position: absolute;
      left: 0;
      width: 100%;
      height: 2px;
      z-index: 2;
      background: var(--primary-green);
      animation: scan 2s linear infinite;

      @keyframes scan {
        0% {
          top: 0;
        }
        50% {
          top: calc(100% - 2px);
        }
        100% {
          top: 0;
        }
      }
    }

    &__success-screen {
      position: absolute;
      display: flex;
      flex-direction: column;
      align-items: center;
      justify-content: center;
      top: 0;
      left: 0;
      width: 100%;
      height: 100%;
      background: var(--vt-c-dark-blue-gray);
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

    &__error-message {
      color: var(--vt-c-light-red);
      margin: 0 0 10px;
      font-size: 0.875rem;
    }

    &__flash-button {
      background: var(--primary-green);
      color: var(--vt-c-dark-blue-gray);
      border: 2px solid var(--primary-green);
      padding: 8px 16px;
      margin-bottom: 10px;
      border-radius: 4px;
      cursor: pointer;
      font-size: 0.875rem;
      font-weight: 500;
      width: 100%;
      transition: 0.2s;

      @media (hover: hover) and (pointer: fine) {
        &:hover {
          background: rgba(137, 225, 89, 0.4);
          color: var(--vt-c-white);
          border: 2px solid var(--primary-green);
          transition: 0.2s;
        }
      }

      &:disabled {
        background: var(--vt-c-light-gray);
        border: 2px solid var(--vt-c-light-gray);
        cursor: not-allowed;
        color: var(--vt-c-black);
      }

      &--on {
        background: var(--vt-c-light-red);
        color: var(--vt-c-white);
        border: 2px solid var(--vt-c-light-red);

        @media (hover: hover) and (pointer: fine) {
          &:hover {
            background-color: rgba(255, 77, 77, 0.4);
            border: 2px solid var(--vt-c-light-red);
            transition: 0.2s;
          }
        }
      }
    }

    &__success-buttons {
      display: flex;
      gap: 10px;
      margin-bottom: 10px;
    }

    &__button {
      flex: 1;
      padding: 8px 16px;
      border-radius: 4px;
      cursor: pointer;
      font-size: 0.875rem;
      font-weight: 600;
      transition: 0.2s;

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
    }
  }
</style>
