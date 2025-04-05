<template>
  <div class="date-picker">
    <div class="date-picker__input" @click="showCalendar = !showCalendar">
      {{ displayValue }}
      <button
        v-if="selectedRange[0]"
        class="date-picker__clear"
        @click.stop="clearSelection"
      >×</button>
    </div>

    <div v-if="showCalendar" class="date-picker__calendar">
      <button class="date-picker__close-mobile" @click="showCalendar = false">×</button>

      <div class="date-picker__header">
        <button class="date-picker__prev" @click="prevMonth">&lt;</button>
        <span>{{ monthName }} {{ currentYear }}</span>
        <button class="date-picker__next" @click="nextMonth">&gt;</button>
      </div>

      <div class="date-picker__days">
        <div
          v-for="day in ['Пн', 'Вт', 'Ср', 'Чт', 'Пт', 'Сб', 'Вс']"
          :key="day"
          class="date-picker__weekday"
        >
          {{ day }}
        </div>

        <div
          v-for="date in calendarDays"
          :key="date.date"
          :class="[
            'date-picker__day',
            { 'date-picker__day--other-month': date.otherMonth },
            { 'date-picker__day--selected': isSelected(date.date) },
            { 'date-picker__day--in-range': isInRange(date.date) },
            { 'date-picker__day--start': isStartDate(date.date) },
            { 'date-picker__day--end': isEndDate(date.date) },
          ]"
          @click="selectDate(date.date)"
          @mouseenter="handleMouseEnter(date.date)"
          @mouseleave="handleMouseLeave"
        >
          {{ new Date(date.date).getDate() }}
        </div>
      </div>

      <div class="date-picker__footer">
        <button class="date-picker__buttons-footer" @click.stop="clearSelection">
          Сброс
        </button>
        <button class="date-picker__buttons-footer" @click="showCalendar = false">
          Отмена
        </button>
      </div>
    </div>
  </div>
</template>


<script lang="ts" setup>
  import { ref, computed, watch, onMounted, onBeforeUnmount } from 'vue';

  const selectionStep = ref<'start' | 'end'>('start');
  const showCalendar = ref(false);
  const currentMonth = ref(new Date().getMonth());
  const currentYear = ref(new Date().getFullYear());
  const hoveredDate = ref<string | null>(null);
  const emit = defineEmits(['update:modelValue']);

  const props = withDefaults(defineProps<{
    modelValue?: [string | null, string | null];
  }>(), {
    modelValue: () => [null, null],
  });

  const selectedRange = ref<[string | null, string | null]>([
    props.modelValue[0],
    props.modelValue[1]
  ]);

  const monthName = computed(() => {
    const months = [
      'Январь', 'Февраль', 'Март', 'Апрель', 'Май', 'Июнь',
      'Июль', 'Август', 'Сентябрь', 'Октябрь', 'Ноябрь', 'Декабрь'
    ];
    return months[currentMonth.value];
  });

  const closeOnOutsideClick = (event: MouseEvent) => {
    const target = event.target as HTMLElement;
    if (!target.closest('.date-picker')) {
      showCalendar.value = false;
    }
  };

  const displayValue = computed(() => {
    const [start, end] = selectedRange.value;
    if (!start) return 'Выберите период';
    if (!end) return formatDate(start);
    return `${formatDate(start)} — ${formatDate(end)}`;
  });

  const formatDateForCalendar = (date: Date) => {
    const year = date.getFullYear();
    const month = String(date.getMonth() + 1).padStart(2, '0');
    const day = String(date.getDate()).padStart(2, '0');
    return `${year}-${month}-${day}`;
  };

  const calendarDays = computed(() => {
    const days = [];
    const date = new Date(currentYear.value, currentMonth.value, 1);

    let firstDay = date.getDay() || 7;
    firstDay -= 1;

    const prevMonthLastDate = new Date(currentYear.value, currentMonth.value, 0).getDate();
    for (let i = firstDay - 1; i >= 0; i--) {
      const prevDate = new Date(currentYear.value, currentMonth.value - 1, prevMonthLastDate - i);
      days.push({
        date: formatDateForCalendar(prevDate),
        otherMonth: true
      });
    }

    const monthDays = new Date(currentYear.value, currentMonth.value + 1, 0).getDate();
    for (let i = 1; i <= monthDays; i++) {
      const currDate = new Date(currentYear.value, currentMonth.value, i);
      days.push({
        date: formatDateForCalendar(currDate),
        otherMonth: false
      });
    }

    const remainingDays = 42 - days.length;
    for (let i = 1; i <= remainingDays; i++) {
      const nextDate = new Date(currentYear.value, currentMonth.value + 1, i);
      days.push({
        date: formatDateForCalendar(nextDate),
        otherMonth: true
      });
    }

    return days;
  });

  const prevMonth = () => {
    if (currentMonth.value === 0) {
      currentMonth.value = 11;
      currentYear.value--;
    } else {
      currentMonth.value--;
    }
  };

  const nextMonth = () => {
    const now = new Date();
    if (currentYear.value < now.getFullYear() || (currentYear.value === now.getFullYear() && currentMonth.value < now.getMonth())) {
      if (currentMonth.value === 11) {
        currentMonth.value = 0;
        currentYear.value++;
      } else {
        currentMonth.value++;
      }
    }
  };

  const handleMouseEnter = (dateString: string) => {
    if (selectionStep.value === 'end' && selectedRange.value[0]) {
      hoveredDate.value = dateString;
    }
  };

  const handleMouseLeave = () => {
    hoveredDate.value = null;
  };

  const formatDate = (dateString: string) => {
    const date = new Date(dateString);
    return date.toLocaleDateString('ru-RU', {
      day: '2-digit',
      month: '2-digit',
      year: 'numeric'
    });
  };

  const selectDate = (dateString: string) => {
    if (selectionStep.value === 'start') {
      selectedRange.value = [dateString, null];
      selectionStep.value = 'end';
    } else {
      const startDate = new Date(selectedRange.value[0] || '');
      const endDate = new Date(dateString);

      if (endDate < startDate) {
        selectedRange.value = [dateString, selectedRange.value[0]];
      } else {
        selectedRange.value = [selectedRange.value[0], dateString];
      }
      selectionStep.value = 'start';
      showCalendar.value = false;
    }

    emit('update:modelValue', selectedRange.value);
  };

  const clearSelection = () => {
    selectedRange.value = [null, null];
    selectionStep.value = 'start';
    emit('update:modelValue', selectedRange.value);
  };

  const isSelected = (dateString: string) => {
    return dateString === selectedRange.value[0] || dateString === selectedRange.value[1];
  };

  const isStartDate = (dateString: string) => {
    return dateString === selectedRange.value[0];
  };

  const isEndDate = (dateString: string) => {
    return dateString === selectedRange.value[1];
  };

  const isInRange = (dateString: string) => {
    const date = new Date(dateString);

    if (selectedRange.value[0] && selectedRange.value[1]) {
      const start = new Date(selectedRange.value[0]);
      const end = new Date(selectedRange.value[1]);
      return date > start && date < end;
    }

    if (selectedRange.value[0] && hoveredDate.value) {
      const start = new Date(selectedRange.value[0]);
      const end = new Date(hoveredDate.value);
      const rangeStart = start < end ? start : end;
      const rangeEnd = start > end ? start : end;
      return date > rangeStart && date < rangeEnd;
    }

    return false;
  };

  watch(() => props.modelValue, ([newStart, newEnd]) => {
    selectedRange.value = [newStart, newEnd];
    if (newStart && !newEnd) {
      selectionStep.value = 'end';
    } else {
      selectionStep.value = 'start';
    }
  }, { deep: true });

  onMounted(() => {
    document.addEventListener('click', closeOnOutsideClick);
  });

  onBeforeUnmount(() => {
    document.removeEventListener('click', closeOnOutsideClick);
  });
</script>

<style lang="scss">
  .date-picker {
    position: relative;

    @media (max-width: 768px) {
      width: 100%;
    }

    &__input {
      width: 100%;
      padding: 10px 16px;
      font-size: 16px;
      background-color: var(--vt-c-light-background);
      border: 1px solid var(--vt-c-blue-gray);
      border-radius: 8px;
      color: var(--vt-c-white);
      cursor: pointer;
      position: relative;
      box-sizing: border-box;
      white-space: nowrap;
      overflow: hidden;
      text-overflow: ellipsis;
      transition: 0.2s;

      @media (hover: hover) and (pointer: fine) {
        &:hover {
          border: 1px solid var(--primary-green);
        }
      }
    }

    &__clear {
      position: absolute;
      right: 10px;
      top: 50%;
      transform: translateY(-50%);
      background: none;
      border: none;
      font-size: 16px;
      color: var(--vt-c-light-gray);
      cursor: pointer;
      transition: 0.2s;

      @media (max-width: 768px) {
        right: 8px;
        font-size: 14px;
      }

      @media (hover: hover) and (pointer: fine) {
        &:hover {
          color: var(--vt-c-light-red);
          transition: 0.2s;
        }
      }
    }

    &__calendar {
      position: absolute;
      top: 100%;
      left: 0;
      z-index: 10;
      width: 300px;
      background-color: var(--vt-c-dark-blue-gray);
      box-shadow:
      0 8px 16px rgba(0, 0, 0, 0.3),
      0 -6px 12px rgba(0, 0, 0, 0.2),
      0 1px 3px rgba(0, 0, 0, 0.25);
      border-radius: 4px;
      padding: 10px;
      margin-top: 5px;
      box-sizing: border-box;

      @media (max-width: 768px) {
        width: 100%;
        position: fixed;
        left: 0;
        right: 0;
        top: auto;
        bottom: 0;
        margin-top: 0;
        border-radius: 12px 12px 0 0;
        box-shadow:
        0 8px 16px rgba(0, 0, 0, 0.3),
        0 -6px 12px rgba(0, 0, 0, 0.2),
        0 1px 3px rgba(0, 0, 0, 0.25);
        padding: 16px;
        padding-top: 42px;
        padding-bottom: calc(16px + env(safe-area-inset-bottom, 0));
      }
    }

    &__header {
      position: relative;
      display: flex;
      justify-content: space-between;
      align-items: center;
      margin-bottom: 10px;
      color: var(--vt-c-white);

      @media (max-width: 768px) {
        margin-bottom: 16px;

        span {
          font-size: 16px;
          font-weight: bold;
        }
      }

      button {
        background: none;
        border: none;
        cursor: pointer;
        font-size: 16px;
        color: var(--vt-c-white);

        @media (max-width: 768px) {
          padding: 8px 12px;
          font-size: 18px;
        }

        @media (hover: hover) and (pointer: fine) {
          &:hover {
            color: var(--primary-green);
          }
        }
      }
    }

    &__close-mobile {
      display: none;

      @media (max-width: 768px) {
        display: block;
        position: absolute;
        top: 10px;
        right: 20px;
        background: none;
        color: var(--vt-c-light-red);
        border: none;
        font-size: 24px;
      }
    }

    &__buttons-footer {
      width: 100%;
      padding: 6px 12px;
      border: 2px solid var(--vt-c-white);
      border-radius: 4px;
      background: none;
      color: var(--vt-c-white);
      transition: 0.2s;

      @media (hover: hover) and (pointer: fine) {
        &:hover {
          border: 2px solid var(--vt-c-light-red);
          color: var(--vt-c-light-red);
          transition: 0.2s;
        }
      }
      @media (max-width: 768px) {
        display: none;
      }
    }

    &__days {
      display: grid;
      grid-template-columns: repeat(7, 1fr);
      gap: 2px;

      @media (max-width: 768px) {
        gap: 4px;
      }
    }

    &__weekday {
      text-align: center;
      font-weight: bold;
      margin-bottom: 5px;
      font-size: 12px;
      color: var(--vt-c-white);

      @media (max-width: 768px) {
        font-size: 13px;
        margin-bottom: 8px;
      }
    }

    &__day {
      height: 30px;
      display: flex;
      align-items: center;
      justify-content: center;
      cursor: pointer;
      border-radius: 4px;
      color: var(--vt-c-white);

      &--other-month {
        color: var(--vt-c-blue-gray);
      }

      &--selected {
        background-color: var(--primary-green);
        color: var(--vt-c-background);
      }

      &--in-range {
        background-color: rgba(137, 225, 89, 0.4);
      }

      &--start {
        border-top-right-radius: 0;
        border-bottom-right-radius: 0;
      }

      &--end {
        border-top-left-radius: 0;
        border-bottom-left-radius: 0;
      }

      @media (max-width: 768px) {
        height: 36px;
        font-size: 14px;
      }

      @media (hover: hover) and (pointer: fine) {
        &:hover {
          background-color: var(--primary-green);
          color: var(--vt-c-background);
        }
      }
    }

    &__footer {
      margin-top: 10px;
      text-align: center;
      display: flex;
      justify-content: space-between;
      gap: 10px;
    }
  }
</style>
