<template>
  <Teleport to="body">
    <Transition name="modal">
      <div v-if="modelValue" class="modal__overlay" @click.self="close">
        <div class="modal">
          <header
            class="modal__header"
            :class="{ 'modal__header--main': currentStep === 'main' }"
          >
            <button
              v-if="currentStep !== 'main'"
              class="modal__back"
              @click="setStep('main')"
            >
              Назад
            </button>
            <button class="modal__close" @click="close">&times;</button>
          </header>

          <div class="modal__body">
            <ModalMain v-if="currentStep === 'main'" @select="setStep" />
            <div v-else class="modal__content">
              <component :is="currentStepComponent" @close="close"/>
            </div>
          </div>
        </div>
      </div>
    </Transition>
  </Teleport>
</template>

<script setup lang="ts">
  import { defineProps, defineEmits, ref, shallowRef } from "vue";
  import ModalMain from "@/components/ModalMain.vue";
  import QrScanner from "@/components/QrScanner.vue";
  import EnterFiscal from "@/components/EnterFiscal.vue";

  import type { Component } from "vue";

  defineProps<{ modelValue: boolean }>();
  const emit = defineEmits(["update:modelValue"]);

  const close = () => {
    emit("update:modelValue", false);
    currentStep.value = "main";
  };

  const currentStep = ref("main");
  const currentStepComponent = shallowRef<Component | null>(null);

  const setStep = (step: string) => {
    currentStep.value = step;
    switch (step) {
      case "scanQR":
        currentStepComponent.value = QrScanner;
        break;
      case "enterFiscal":
      case "enterReceipt":
      case "uploadPhoto":
        currentStepComponent.value = EnterFiscal;
        break;
      default:
        currentStepComponent.value = null;
    }
  };
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
    max-height: 80vh;
    max-width: 80vw;

    &__overlay {
      position: fixed;
      inset: 0;
      background: rgba(0, 0, 0, 0.6);
      display: flex;
      align-items: center;
      justify-content: center;
      z-index: 1000;
      height: 100vh;
      width: 100vw;
    }

    &__header {
      display: flex;
      justify-content: space-between;
      align-items: center;

      &--main {
        justify-content: end;
      }
    }

    &__close {
      background: none;
      border: none;
      font-size: 30px;
      color: inherit;
      font-weight: 500;
    }

    &__body {
      display: flex;
      flex-direction: column;
      flex-grow: 1;
      min-height: 0;
    }

    &__content {
      // border-radius: 12px;
      width: min(400px, 80vw);
      display: flex;
      flex-direction: column;
      overflow: hidden;
    }

    &__back {
      align-items: center;
      background: var(--primary-green);
      color: var(--vt-c-dark-blue-gray);
      border-radius: 5px;
      padding: 5px;
      border: 2px solid transparent;
      box-sizing: border-box;
      transition: 0.2s;
      font-weight: 500;
      font-size: 16px;
      transition: 0.2;
      @media (hover: hover) and (pointer: fine) {
        &:hover {
          background: rgba(137, 225, 89, 0.4);
          color: var(--vt-c-white);
          border: 2px solid var(--primary-green);
          transition: 0.2s;
        }
      }
    }

    &-enter-active,
    &-leave-active {
      transition: opacity 0.3s ease;
    }

    &-enter-from,
    &-leave-to {
      opacity: 0;
    }
  }
</style>
