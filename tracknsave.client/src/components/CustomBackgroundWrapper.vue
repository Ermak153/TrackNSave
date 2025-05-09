<template>
  <TransformableImage
    :touch-move="touchMove"
    :touch-resize="touchResize"
    :mouse-move="mouseMove"
    :wheel-resize="wheelResize"
    :events-filter="eventsFilter"
    @move="emit('move', $event)"
    @resize="emit('resize', $event)"
  >
    <slot></slot>
    <div
      class="cropper-event-notification"
      :class="{ 'cropper-event-notification--visible': notificationVisible }"
    >
      {{
        notificationType === 'wheel'
          ? "Используйте Ctrl + прокрутку, чтобы увеличить или уменьшить область обрезки"
          : "Используйте два пальца, чтобы переместить область обрезки"
      }}
    </div>
  </TransformableImage>
</template>

<script setup lang="ts">
import { ref, onMounted, onBeforeUnmount } from 'vue';
import { TransformableImage } from 'vue-advanced-cropper';
import debounce from 'debounce';

type DebouncedFunction = {
  (): void;
  clear(): void;
};

defineProps<{
  touchMove?: boolean;
  mouseMove?: boolean;
  touchResize?: boolean;
  wheelResize?: boolean;
}>();


const emit = defineEmits<{
  (e: 'move', event: Event): void;
  (e: 'resize', event: Event): void;
}>();

const notificationVisible = ref(false);
const notificationType = ref<'touch' | 'wheel' | null>(null);
let hideNotifications: DebouncedFunction;

onMounted(() => {
  hideNotifications = debounce(() => {
    notificationVisible.value = false;
  }, 1000);
});

onBeforeUnmount(() => {
  hideNotifications?.clear();
});

const eventsFilter = (nativeEvent: Event, transforming: boolean) => {
  const event = nativeEvent as TouchEvent | WheelEvent;

  if (event.type === 'touchstart' || event.type === 'touchmove') {
    if ((event as TouchEvent).touches?.length === 1 && !transforming) {
      notificationVisible.value = true;
      notificationType.value = 'touch';
      hideNotifications?.();
      return false;
    }
    notificationVisible.value = false;
  } else if (event.type === 'wheel') {
    if (!transforming && !(event as WheelEvent).ctrlKey) {
      notificationVisible.value = true;
      notificationType.value = 'wheel';
      hideNotifications?.();
      return false;
    }
    notificationVisible.value = false;
  }

  event.preventDefault();
  event.stopPropagation();
};
</script>

<style lang="scss">
.cropper-event-notification {
  background: rgba(0, 0, 0, 0.6);
  color: white;
  position: absolute;
  left: 0;
  top: 0;
  bottom: 0;
  right: 0;
  display: flex;
  justify-content: center;
  align-items: center;
  text-align: center;
  font-size: 20px;
  transition: opacity 0.5s;
  opacity: 0;
  pointer-events: none;
  padding-left: 50px;
  padding-right: 50px;

  &--visible {
    transition: opacity 0.25s;
    pointer-events: all;
    opacity: 1;
  }
}
</style>
