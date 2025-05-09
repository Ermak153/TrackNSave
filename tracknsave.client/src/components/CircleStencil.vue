<script setup lang="ts">

import { computed } from 'vue';
import {
  DraggableElement,
  DraggableArea,
  StencilPreview,
  ResizeEvent,
} from 'vue-advanced-cropper';

// Определяем кастомные типы
interface Transition {
  enabled?: boolean;
  time?: number;
  timingFunction?: string;
}

interface Coordinates {
  width: number;
  height: number;
  left: number;
  top: number;
}

interface ImageType {
  src: string;
  width: number;
  height: number;
}

interface Props {
  image?: ImageType;
  coordinates?: Coordinates;
  transitions?: Transition;
  stencilCoordinates?: Coordinates;
}

const props = defineProps<Props>();

const emit = defineEmits<{
  (e: 'move', event: unknown): void;
  (e: 'move-end'): void;
  (e: 'resize', event: ResizeEvent): void;
  (e: 'resize-end'): void;
}>();

const style = computed(() => {
  if (!props.stencilCoordinates) return {};
  const { height, width, left, top } = props.stencilCoordinates;
  return {
    width: `${width}px`,
    height: `${height}px`,
    transform: `translate(${left}px, ${top}px)`,
    transition: props.transitions?.enabled
      ? `${props.transitions.time}ms ${props.transitions.timingFunction}`
      : undefined
  };
});

interface ResizeDragEvent {
  shift: () => { left: number; top: number };
}

const onMove = (moveEvent: unknown) => {
  emit('move', moveEvent);
};

const onMoveEnd = () => {
  emit('move-end');
};

const onResize = (dragEvent: ResizeDragEvent) => {
  const shift = dragEvent.shift();
  emit(
    'resize',
    new ResizeEvent(
      {
        left: shift.left,
        right: shift.left,
        top: -shift.top,
        bottom: -shift.top,
      },
      { compensate: true }
    )
  );
};

const onResizeEnd = () => {
  emit('resize-end');
};
</script>

<template>
  <div class="circle-stencil" :style="style">
    <DraggableElement
      class="circle-stencil__handler"
      @drag="onResize"
      @drag-end="onResizeEnd"
    >
      <svg
        class="circle-stencil__icon"
        xmlns="http://www.w3.org/2000/svg"
        width="26.7"
        height="26.3"
        @mousedown.prevent
      >
        <path
          fill="#FFF"
          d="M15.1 4.7L18.3 6l-3.2 3.3 2.3 2.3 3.3-3.3 1.3 3.3L26.7 0zM9.3 14.7L6 18l-1.3-3.3L0 26.3l11.6-4.7-3.3-1.3 3.3-3.3z"
        />
      </svg>
    </DraggableElement>
    <DraggableArea @move="onMove" @move-end="onMoveEnd">
      <StencilPreview
        class="circle-stencil__preview"
        :image="image"
        :coordinates="coordinates"
        :width="stencilCoordinates?.width"
        :height="stencilCoordinates?.height"
        :transitions="transitions"
      />
    </DraggableArea>
  </div>
</template>

<style lang="scss">
.circle-stencil {
  border-radius: 50%;
  cursor: move;
  position: absolute;
  border: dashed 2px white;
  box-sizing: border-box;

  &__handler {
    position: absolute;
    right: 15%;
    top: 14%;
    z-index: 1;
    cursor: ne-resize;
    width: 30px;
    height: 30px;
    display: flex;
    align-items: center;
    justify-content: center;
    transform: translate(50%, -50%);
  }

  &__preview {
    border-radius: 50%;
    overflow: hidden;
  }
}
</style>
