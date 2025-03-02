<template>
  <div class="datetime-picker" ref="pickerRef">
    <label v-if="label" :for="id" class="datetime-picker__label">{{ label }}</label>
    <div class="datetime-picker__input-wrapper">
      <input
        :id="id"
        ref="input"
        v-model="displayValue"
        type="text"
        class="datetime-picker__input"
        :placeholder="placeholder"
        @focus="onFocus"
        @blur="onBlur"
        @input="onInput"
        @keydown="onKeyDown"
        :disabled="disabled"
      />
      <div class="datetime-picker__icon" @click="toggleCalendar" v-if="!disabled">
        <svg width="18" height="18" viewBox="0 0 24 24">
          <path d="M20,3H19V1H17V3H7V1H5V3H4A2,2 0 0,0 2,5V21A2,2 0 0,0 4,23H20A2,2 0 0,0 22,21V5A2,2 0 0,0 20,3M20,21H4V8H20V21Z" />
        </svg>
      </div>
    </div>
    <div v-if="isCalendarVisible" class="datetime-picker__calendar">
      <div class="datetime-picker__calendar-header">
        <button @click.stop="prevMonth">&lt;</button>
        <span>{{ currentMonthName }} {{ currentYear }}</span>
        <button @click.stop="nextMonth">&gt;</button>
      </div>
      <div class="datetime-picker__calendar-days">
        <div v-for="day in ['Пн', 'Вт', 'Ср', 'Чт', 'Пт', 'Сб', 'Вс']" :key="day" class="day-name">{{ day }}</div>
        <div
          v-for="(day, index) in calendarDays"
          :key="index"
          class="day"
          :class="{
            'other-month': day.otherMonth,
            'selected': day.selected,
            'today': day.today
          }"
          @mousedown.prevent
          @click.stop="selectDate(day)"
        >
          {{ day.day }}
        </div>
      </div>
      <div class="datetime-picker__time">
        <input
          type="text"
          v-model="timeValue"
          placeholder="00:00"
          @input="formatTimeInput"
          @mousedown.stop
        />
      </div>
      <div class="datetime-picker__buttons">
        <button @click.stop="applyDateTime" class="datetime-picker__apply">Применить</button>
        <button @click.stop="closeCalendar" class="datetime-picker__cancel">Отмена</button>
      </div>
    </div>
    <div v-if="error" class="datetime-picker__error">{{ error }}</div>
  </div>
</template>

<script lang="ts" setup>
import { ref, computed, onMounted, watch, onBeforeUnmount } from 'vue';

interface Props {
  modelValue?: Date;
  label?: string;
  placeholder?: string;
  disabled?: boolean;
  id?: string;
}

const props = withDefaults(defineProps<Props>(), {
  modelValue: undefined,
  label: '',
  placeholder: 'ДД.ММ.ГГГГ ЧЧ:ММ',
  disabled: false,
  id: 'datetime-picker'
});

const emit = defineEmits(['update:modelValue', 'change']);

const input = ref<HTMLInputElement | null>(null);
const pickerRef = ref<HTMLElement | null>(null);
const displayValue = ref('');
const timeValue = ref('');
const isCalendarVisible = ref(false);
const error = ref('');
const selectedDate = ref<Date | null>(props.modelValue || null);
const tempSelectedDate = ref<Date | null>(null);

const currentDate = ref(new Date());
const currentYear = computed(() => currentDate.value.getFullYear());
const currentMonth = computed(() => currentDate.value.getMonth());
const currentMonthName = computed(() => {
  const months = ['Январь', 'Февраль', 'Март', 'Апрель', 'Май', 'Июнь', 'Июль', 'Август', 'Сентябрь', 'Октябрь', 'Ноябрь', 'Декабрь'];
  return months[currentMonth.value];
});

// Вычисляем дни для отображения в календаре
const calendarDays = computed(() => {
  const days = [];
  const firstDay = new Date(currentYear.value, currentMonth.value, 1);
  const lastDay = new Date(currentYear.value, currentMonth.value + 1, 0);

  // Получаем день недели для первого дня месяца (0 - воскресенье, 1 - понедельник)
  let firstDayOfWeek = firstDay.getDay();
  // Корректируем для начала недели с понедельника (0 - понедельник, 6 - воскресенье)
  firstDayOfWeek = firstDayOfWeek === 0 ? 6 : firstDayOfWeek - 1;

  // Добавляем дни предыдущего месяца
  const prevMonthLastDay = new Date(currentYear.value, currentMonth.value, 0).getDate();
  for (let i = firstDayOfWeek; i > 0; i--) {
    const date = new Date(currentYear.value, currentMonth.value - 1, prevMonthLastDay - i + 1);
    days.push({
      day: prevMonthLastDay - i + 1,
      date,
      otherMonth: true,
      selected: isDateSameDay(date, tempSelectedDate.value || selectedDate.value),
      today: false
    });
  }

  // Добавляем дни текущего месяца
  const today = new Date();
  for (let i = 1; i <= lastDay.getDate(); i++) {
    const date = new Date(currentYear.value, currentMonth.value, i);
    days.push({
      day: i,
      date,
      otherMonth: false,
      selected: isDateSameDay(date, tempSelectedDate.value || selectedDate.value),
      today: today.getDate() === i &&
             today.getMonth() === currentMonth.value &&
             today.getFullYear() === currentYear.value
    });
  }

  // Добавляем дни следующего месяца до заполнения сетки
  const daysNeeded = 42 - days.length;
  for (let i = 1; i <= daysNeeded; i++) {
    const date = new Date(currentYear.value, currentMonth.value + 1, i);
    days.push({
      day: i,
      date,
      otherMonth: true,
      selected: isDateSameDay(date, tempSelectedDate.value || selectedDate.value),
      today: false
    });
  }

  return days;
});

// Функция для сравнения дат (без времени)
function isDateSameDay(date1: Date | null, date2: Date | null): boolean {
  if (!date1 || !date2) return false;

  return date1.getDate() === date2.getDate() &&
         date1.getMonth() === date2.getMonth() &&
         date1.getFullYear() === date2.getFullYear();
}

// Методы для работы с календарем
const prevMonth = () => {
  currentDate.value = new Date(currentYear.value, currentMonth.value - 1, 1);
};

const nextMonth = () => {
  currentDate.value = new Date(currentYear.value, currentMonth.value + 1, 1);
};

const selectDate = (day: { date: Date }) => {
  // Создаем новую дату, чтобы не модифицировать оригинал
  const newDate = new Date(day.date);

  // Сохраняем текущее время или устанавливаем 00:00
  if (tempSelectedDate.value) {
    newDate.setHours(tempSelectedDate.value.getHours());
    newDate.setMinutes(tempSelectedDate.value.getMinutes());
  } else if (selectedDate.value) {
    newDate.setHours(selectedDate.value.getHours());
    newDate.setMinutes(selectedDate.value.getMinutes());
  } else {
    newDate.setHours(0);
    newDate.setMinutes(0);
  }

  // Устанавливаем временную выбранную дату
  tempSelectedDate.value = newDate;

  // Обновляем значение времени
  timeValue.value = formatTime(newDate);

  // Обновляем отображаемое значение, чтобы показать выбранную дату в поле ввода
  updateTempDisplayValue();
};

// Обновляет displayValue на основе tempSelectedDate
const updateTempDisplayValue = () => {
  if (tempSelectedDate.value) {
    const day = tempSelectedDate.value.getDate().toString().padStart(2, '0');
    const month = (tempSelectedDate.value.getMonth() + 1).toString().padStart(2, '0');
    const year = tempSelectedDate.value.getFullYear();
    const hours = tempSelectedDate.value.getHours().toString().padStart(2, '0');
    const minutes = tempSelectedDate.value.getMinutes().toString().padStart(2, '0');

    displayValue.value = `${day}.${month}.${year} ${hours}:${minutes}`;
  }
};

// Функция для форматирования времени
const formatTime = (date: Date): string => {
  const hours = date.getHours().toString().padStart(2, '0');
  const minutes = date.getMinutes().toString().padStart(2, '0');
  return `${hours}:${minutes}`;
};

// Обработка ввода времени
const formatTimeInput = () => {
  // Удаляем все нецифровые символы, кроме двоеточия
  let value = timeValue.value.replace(/[^\d:]/g, '');

  // Форматируем время
  if (value.length > 0) {
    const parts = value.split(':');
    let hours = parts[0] || '';
    let minutes = parts[1] || '';

    // Ограничиваем часы до 23
    if (hours.length > 0) {
      let hoursNum = parseInt(hours);
      if (hoursNum > 23) hoursNum = 23;
      hours = hoursNum.toString().padStart(2, '0');
    }

    // Ограничиваем минуты до 59
    if (minutes.length > 0) {
      let minutesNum = parseInt(minutes);
      if (minutesNum > 59) minutesNum = 59;
      minutes = minutesNum.toString().padStart(2, '0');
    }

    if (hours.length > 0) {
      value = hours;
      if (minutes.length > 0) {
        value += ':' + minutes;
      } else if (value.includes(':')) {
        value += ':';
      }
    }
  }

  timeValue.value = value;

  // Обновляем выбранную дату со временем, если дата уже выбрана
  if (tempSelectedDate.value && value.includes(':')) {
    const [hours, minutes] = value.split(':').map(part => parseInt(part) || 0);
    const newDate = new Date(tempSelectedDate.value);
    newDate.setHours(hours);
    newDate.setMinutes(minutes);
    tempSelectedDate.value = newDate;

    // Обновляем отображаемое значение
    updateTempDisplayValue();
  }
};

const toggleCalendar = () => {
  if (!props.disabled) {
    isCalendarVisible.value = !isCalendarVisible.value;

    if (isCalendarVisible.value) {
      // Инициализируем временный выбор на основе текущего выбора или текущей даты
      if (selectedDate.value) {
        tempSelectedDate.value = new Date(selectedDate.value);
        currentDate.value = new Date(
          selectedDate.value.getFullYear(),
          selectedDate.value.getMonth(),
          1
        );
        timeValue.value = formatTime(selectedDate.value);
      } else {
        const now = new Date();
        tempSelectedDate.value = now;
        currentDate.value = new Date(
          now.getFullYear(),
          now.getMonth(),
          1
        );
        timeValue.value = formatTime(now);
      }
    }
  }
};

const closeCalendar = () => {
  isCalendarVisible.value = false;
  tempSelectedDate.value = null;

  // Восстанавливаем отображение выбранной даты, если она есть
  if (selectedDate.value) {
    updateDisplayValue();
  }
};

// Применяем выбранную дату
const applyDateTime = () => {
  if (tempSelectedDate.value) {
    selectedDate.value = new Date(tempSelectedDate.value);
    emit('update:modelValue', selectedDate.value);
    emit('change', selectedDate.value);
    updateDisplayValue();
    closeCalendar();
  }
};

// Функция для обработки ввода с маской (оставляем без изменений, т.к. она работает хорошо)
const onInput = (event: Event) => {
  const target = event.target as HTMLInputElement;
  const value = target.value;

  // Получаем только цифры из введенного значения
  const digits = value.replace(/\D/g, '');

  // Применяем базовую маску для ввода
  let formattedValue = '';

  if (digits.length > 0) {
    // День
    formattedValue += digits.substring(0, Math.min(2, digits.length));

    if (digits.length > 2) {
      formattedValue += '.';
      // Месяц
      formattedValue += digits.substring(2, Math.min(4, digits.length));

      if (digits.length > 4) {
        formattedValue += '.';
        // Год
        formattedValue += digits.substring(4, Math.min(8, digits.length));

        if (digits.length > 8) {
          formattedValue += ' ';
          // Час
          formattedValue += digits.substring(8, Math.min(10, digits.length));

          if (digits.length > 10) {
            formattedValue += ':';
            // Минуты
            formattedValue += digits.substring(10, Math.min(12, digits.length));
          }
        }
      }
    }
  }

  // Устанавливаем отформатированное значение
  displayValue.value = formattedValue;

  // Сохраняем позицию курсора
  const cursorPos = target.selectionStart || 0;

  // Вычисляем новую позицию курсора
  setTimeout(() => {
    if (input.value) {
      let newPos = cursorPos;

      // Если добавилось больше символов из-за форматирования,
      // сдвигаем курсор вперед на разницу
      const diffLen = formattedValue.length - value.length;
      newPos += Math.max(0, diffLen);

      // Ограничиваем по длине строки
      newPos = Math.min(newPos, formattedValue.length);

      input.value.selectionStart = newPos;
      input.value.selectionEnd = newPos;
    }
  }, 0);

  // Пытаемся распарсить дату и обновить модель
  if (formattedValue.length >= 16) { // ДД.ММ.ГГГГ ЧЧ:ММ = 16 символов
    const parts = formattedValue.split(/[\.\s\:]/);
    if (parts.length === 5) {
      const day = parseInt(parts[0]);
      const month = parseInt(parts[1]) - 1; // В JavaScript месяцы от 0 до 11
      const year = parseInt(parts[2]);
      const hours = parseInt(parts[3]);
      const minutes = parseInt(parts[4]);

      // Проверка валидности даты
      if (isValidDate(day, month, year, hours, minutes)) {
        const newDate = new Date(year, month, day, hours, minutes);

        // Обновляем временное значение даты
        tempSelectedDate.value = newDate;

        // Если календарь открыт, обновляем отображаемый месяц
        if (isCalendarVisible.value) {
          currentDate.value = new Date(year, month, 1);
          timeValue.value = formatTime(newDate);
        }

        error.value = '';
      }
    }
  }
};

// Функция проверки валидности даты
function isValidDate(day: number, month: number, year: number, hours: number, minutes: number): boolean {
  // Базовая валидация
  if (month < 0 || month > 11) {
    error.value = 'Месяц должен быть от 1 до 12';
    return false;
  }

  if (day < 1) {
    error.value = 'День должен быть положительным числом';
    return false;
  }

  // Проверка валидности дня для выбранного месяца и года
  const lastDayOfMonth = new Date(year, month + 1, 0).getDate();
  if (day > lastDayOfMonth) {
    error.value = `День должен быть от 1 до ${lastDayOfMonth} для выбранного месяца`;
    return false;
  }

  if (hours < 0 || hours > 23) {
    error.value = 'Часы должны быть от 0 до 23';
    return false;
  }

  if (minutes < 0 || minutes > 59) {
    error.value = 'Минуты должны быть от 0 до 59';
    return false;
  }

  return true;
}

const onKeyDown = (e: KeyboardEvent) => {
  // Разрешаем навигационные клавиши
  if (['ArrowLeft', 'ArrowRight', 'ArrowUp', 'ArrowDown', 'Tab', 'Enter', 'Backspace', 'Delete'].includes(e.key)) {
    return;
  }

  // Разрешаем только цифры
  if (!/\d/.test(e.key)) {
    e.preventDefault();
    return;
  }

  // Ограничиваем максимальную длину
  if (displayValue.value.replace(/\D/g, '').length >= 12 &&
      !(input.value?.selectionStart !== input.value?.selectionEnd)) {
    e.preventDefault();
    return;
  }
};

const onFocus = () => {
  if (!props.disabled) {
    // Показываем календарь при фокусе
    isCalendarVisible.value = true;

    // Если у нас есть дата, устанавливаем временное значение
    if (selectedDate.value) {
      tempSelectedDate.value = new Date(selectedDate.value);
      currentDate.value = new Date(
        selectedDate.value.getFullYear(),
        selectedDate.value.getMonth(),
        1
      );
      timeValue.value = formatTime(selectedDate.value);
    }
  }
};

const onBlur = (event: FocusEvent) => {
  // Проверяем, что клик не был по календарю
  const relatedTarget = event.relatedTarget as Element;
  if (relatedTarget && pickerRef.value && pickerRef.value.contains(relatedTarget)) {
    return;
  }

  // Пытаемся применить введенную дату
  if (displayValue.value && displayValue.value.length >= 16) {
    const parts = displayValue.value.split(/[\.\s\:]/);
    if (parts.length === 5) {
      const day = parseInt(parts[0]);
      const month = parseInt(parts[1]) - 1;
      const year = parseInt(parts[2]);
      const hours = parseInt(parts[3]);
      const minutes = parseInt(parts[4]);

      if (isValidDate(day, month, year, hours, minutes)) {
        const newDate = new Date(year, month, day, hours, minutes);

        // Применяем дату
        selectedDate.value = newDate;
        emit('update:modelValue', selectedDate.value);
        emit('change', selectedDate.value);
        updateDisplayValue();
      }
    }
  }

  // Закрываем календарь при потере фокуса, если не клик по календарю
  setTimeout(() => {
    if (!pickerRef.value?.contains(document.activeElement)) {
      closeCalendar();
    }
  }, 100);
};

// Обработчик клика вне компонента
const handleClickOutside = (event: MouseEvent) => {
  // Проверяем, что клик был вне компонента
  if (pickerRef.value && !pickerRef.value.contains(event.target as Node) && isCalendarVisible.value) {
    // При клике вне компонента применим текущий выбор
    if (tempSelectedDate.value) {
      selectedDate.value = new Date(tempSelectedDate.value);
      emit('update:modelValue', selectedDate.value);
      emit('change', selectedDate.value);
      updateDisplayValue();
    }
    closeCalendar();
  }
};

// Обновление отображаемого значения на основе выбранной даты
const updateDisplayValue = () => {
  if (selectedDate.value) {
    const day = selectedDate.value.getDate().toString().padStart(2, '0');
    const month = (selectedDate.value.getMonth() + 1).toString().padStart(2, '0');
    const year = selectedDate.value.getFullYear();
    const hours = selectedDate.value.getHours().toString().padStart(2, '0');
    const minutes = selectedDate.value.getMinutes().toString().padStart(2, '0');

    displayValue.value = `${day}.${month}.${year} ${hours}:${minutes}`;

    // Обновляем время в поле ввода времени
    if (isCalendarVisible.value) {
      timeValue.value = `${hours}:${minutes}`;
    }
  } else {
    displayValue.value = '';
    timeValue.value = '';
  }
};

// Инициализация значения при загрузке компонента
onMounted(() => {
  if (props.modelValue) {
    selectedDate.value = new Date(props.modelValue);
    updateDisplayValue();
  }

  // Добавляем обработчик клика вне компонента
  document.addEventListener('mousedown', handleClickOutside);
});

// Удаляем обработчик при удалении компонента
onBeforeUnmount(() => {
  document.removeEventListener('mousedown', handleClickOutside);
});

// Следим за изменениями modelValue
watch(() => props.modelValue, (newValue) => {
  if (newValue) {
    selectedDate.value = new Date(newValue);
    updateDisplayValue();
  } else {
    selectedDate.value = null;
    displayValue.value = '';
    tempSelectedDate.value = null;
  }
});
</script>

<style lang="scss">
.datetime-picker {
  position: relative;
  font-family: Arial, sans-serif;
  width: 100%;

  &__label {
    display: block;
    margin-bottom: 5px;
    font-weight: 500;
    font-size: 14px;
  }

  &__input-wrapper {
    position: relative;
    display: flex;
    align-items: center;
  }

  &__input {
    width: 100%;
    padding: 8px 12px;
    font-size: 14px;
    border: 1px solid #ccc;
    border-radius: 4px;
    outline: none;
    transition: border-color 0.3s;

    &:focus {
      border-color: #4a90e2;
      box-shadow: 0 0 0 2px rgba(74, 144, 226, 0.2);
    }

    &:disabled {
      background-color: #f5f5f5;
      cursor: not-allowed;
    }
  }

  &__icon {
    position: absolute;
    right: 10px;
    top: 50%;
    transform: translateY(-50%);
    cursor: pointer;
    color: #666;

    svg {
      fill: currentColor;
    }
  }

  &__calendar {
    position: absolute;
    top: 100%;
    left: 0;
    z-index: 10;
    width: 300px;
    background: white;
    border: 1px solid #ccc;
    border-radius: 4px;
    box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
    margin-top: 5px;
    padding: 12px;
  }

  &__calendar-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
    margin-bottom: 10px;

    button {
      background: none;
      border: none;
      cursor: pointer;
      font-size: 16px;
      color: #4a90e2;
      padding: 4px 8px;

      &:hover {
        background-color: #f5f5f5;
        border-radius: 4px;
      }
    }

    span {
      font-weight: bold;
    }
  }

  &__calendar-days {
    display: grid;
    grid-template-columns: repeat(7, 1fr);
    gap: 5px;

    .day-name {
      text-align: center;
      font-weight: bold;
      font-size: 12px;
      padding: 5px 0;
      color: #666;
    }

    .day {
      display: flex;
      align-items: center;
      justify-content: center;
      height: 32px;
      cursor: pointer;
      border-radius: 4px;
      font-size: 14px;

      &:hover {
        background-color: #f0f0f0;
      }

      &.selected {
        background-color: #4a90e2;
        color: white;
      }

      &.today {
        font-weight: bold;
        border: 1px solid #4a90e2;
      }

      &.other-month {
        color: #aaa;
      }
    }
  }

  &__time {
    margin-top: 10px;
    padding: 10px 0;
    display: flex;
    justify-content: center;

    input {
      width: 80px;
      padding: 8px;
      text-align: center;
      border: 1px solid #ccc;
      border-radius: 4px;
      font-size: 14px;

      &:focus {
        border-color: #4a90e2;
        outline: none;
      }
    }
  }

  &__buttons {
    display: flex;
    justify-content: space-between;
    margin-top: 10px;

    button {
      padding: 8px 12px;
      border: none;
      border-radius: 4px;
      cursor: pointer;
      font-size: 14px;

      &.datetime-picker__apply {
        background-color: #4a90e2;
        color: white;

        &:hover {
          background-color: darken(#4a90e2, 10%);
        }
      }

      &.datetime-picker__cancel {
        background-color: #f5f5f5;
        color: #333;

        &:hover {
          background-color: darken(#f5f5f5, 5%);
        }
      }
    }
  }

  &__error {
    color: #e74c3c;
    font-size: 12px;
    margin-top: 5px;
  }
}
</style>
