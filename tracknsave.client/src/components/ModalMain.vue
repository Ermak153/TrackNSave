<template>
  <div class="modal-main">
    <button
      v-for="option in options"
      :key="option.key"
      @click="selectOption(option.key)"
      class="modal-main__button"
    >
      <img :src="option.icon" :alt="`Иконка ${option.label}`" />
      {{ option.label }}
    </button>
  </div>
</template>

<script setup lang="ts">
  import { defineEmits } from "vue";
  import qrcode from "@/assets/icons/qr-code.svg";
  import fd from "@/assets/icons/fd.svg";
  import receipt from "@/assets/icons/receipt.svg";
  import upload from "@/assets/icons/upload.svg";

  const emit = defineEmits(["select"]);

  const options = [
    { key: "scanQR", label: "Сканировать QR-код", icon: qrcode },
    { key: "enterFiscal", label: "Ввести фискальные данные", icon: fd },
    { key: "enterReceipt", label: "Ввести чек вручную", icon: receipt },
    { key: "uploadPhoto", label: "Загрузить фото", icon: upload },
  ];

  const selectOption = (key: string) => {
    emit("select", key);
  };
</script>

<style lang="scss" scoped>
  .modal-main {
    display: flex;
    flex-direction: column;
    justify-content: flex-start;
    align-items: center;
    gap: 10px;
    width: 400px;
    max-width: 100%;
    overflow-y: auto;
    flex-grow: 1;

    @media (max-width: 768px) {
      width: 300px;
    }

    &__button {
      height: 70px;
      width: 100%;
      display: grid;
      grid-template-columns: 40px 1fr;
      align-items: center;
      gap: 8px;
      background: var(--vt-c-dark-green);
      color: var(--vt-c-white);
      padding: 10px 15px;
      padding-left: 20px;
      border-radius: 5px;
      border: 2px solid transparent;
      font-size: 16px;
      margin-top: 10px;
      box-sizing: border-box;
      transition: 0.2s;
      font-weight: 500;

      img {
        width: 24px;
        height: 24px;
        justify-self: center;
      }

      @media (hover: hover) and (pointer: fine) {
        &:hover {
          background: rgba(137, 225, 89, 0.2);
          border: 2px solid var(--vt-c-dark-green);
          transition: 0.2s;
          color: var(--vt-c-white);
        }
      }
    }
  }
</style>
