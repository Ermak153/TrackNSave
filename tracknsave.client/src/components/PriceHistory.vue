<template>
  <Teleport to="body">
    <Transition name="overlay">
      <div v-if="visible" class="price-history__overlay" @click.self="close">
        <Transition name="modal">
          <div v-if="visible" class="price-history">
            <header class="price-history__header">
              <h2 class="price-history__title">{{ productName }}</h2>
              <button class="price-history__close" @click="close">
                &times;
              </button>
            </header>

            <div class="price-history__body">
              <div class="price-history__content">
                <div ref="chartRef" class="price-history__chart"></div>
              </div>
            </div>
          </div>
        </Transition>
      </div>
    </Transition>
  </Teleport>
</template>

<script lang="ts" setup>
  import { ref, watch, onBeforeUnmount, nextTick } from "vue";
  import * as echarts from "echarts";
  import { useToast } from "@/composables/useToast";

  const toast = useToast();
  const props = defineProps<Props>();
  const emit = defineEmits(["update:modelValue"]);
  const visible = ref(props.modelValue);
  const chartRef = ref<HTMLElement | null>(null);
  let chart: echarts.ECharts | null = null;
  let resizeObserver: ResizeObserver | null = null;

  const chartColors = {
    line: "#89E159",
    text: "#FFFFFF",
    axisLine: "#5C6A85",
    tooltip: {
      background: "#1F2937",
      text: "#FFFFFF",
      border: "#89E159",
    },
    gridLine: "rgba(92, 106, 133, 0.2)",
  };

  interface PriceEntry {
    price: number;
    dateTime: string;
    retailPlace: string;
  }

  interface Props {
    modelValue: boolean;
    productName: string;
    priceHistory: PriceEntry[];
  }

  interface TooltipParam {
    dataIndex: number;
    seriesName: string;
    data: number;
    value: number;
    axisValue: string;
    marker: string;
  }

  const initChart = async () => {
    if (!chartRef.value) return;

    try {
      disposeChart();

      echarts.registerTheme("customTheme", {
        color: [chartColors.line],
        backgroundColor: "transparent",
        textStyle: { color: chartColors.text },
        line: {
          itemStyle: { borderWidth: 2 },
          lineStyle: { width: 3 },
          symbolSize: 8,
          symbol: "circle",
          smooth: true,
        },
        tooltip: {
          backgroundColor: chartColors.tooltip.background,
          textStyle: { color: chartColors.tooltip.text },
          borderColor: chartColors.tooltip.border,
          borderWidth: 1,
        },
      });

      chart = echarts.init(chartRef.value, "customTheme");

      resizeObserver = new ResizeObserver(() => {
        chart?.resize();
      });
      resizeObserver.observe(chartRef.value);

      updateChart();

      setTimeout(() => chart?.resize(), 300);
    } catch {
      toast.show("Ошибка загрузки графика", "error");
    }
  };

  const updateChart = () => {
    if (!chart || !props.priceHistory.length) return;

    const sorted = [...props.priceHistory].sort(
      (a, b) => new Date(a.dateTime).getTime() - new Date(b.dateTime).getTime()
    );

    const dates = sorted.map((e) => new Date(e.dateTime).toLocaleDateString());
    const values = sorted.map((e) => Number(e.price) / 100);

    chart.setOption({
      grid: {
        left: "5%",
        right: "5%",
        bottom: "15%",
        top: "10%",
        containLabel: true,
      },
      tooltip: {
        trigger: "axis",
        formatter: (params: TooltipParam[]) => {
          const idx = params[0].dataIndex;
          const item = sorted[idx];
          return [
            `<div style="font-weight:bold">${item.retailPlace}</div>`,
            `<div>${new Date(item.dateTime).toLocaleString()}</div>`,
            `<div>Цена: ${(Number(item.price) / 100).toFixed(2)} ₽</div>`,
          ].join("");
        },
      },
      xAxis: {
        type: "category",
        data: dates,
        axisLine: { lineStyle: { color: chartColors.axisLine } },
        axisLabel: {
          rotate: 45,
          color: chartColors.text,
          margin: 15,
        },
      },
      yAxis: {
        type: "value",
        min: Math.floor(Math.min(...values) * 0.95),
        axisLine: { lineStyle: { color: chartColors.axisLine } },
        axisLabel: { formatter: "{value} ₽", color: chartColors.text },
      },
      series: [
        {
          name: props.productName,
          type: "line",
          data: values,
          smooth: true,
          symbolSize: 8,
          lineStyle: { width: 3, color: chartColors.line },
          areaStyle: {
            color: new echarts.graphic.LinearGradient(0, 0, 0, 1, [
              { offset: 0, color: "rgba(137, 225, 89, 0.3)" },
              { offset: 1, color: "rgba(137, 225, 89, 0.05)" },
            ]),
          },
        },
      ],
    });
  };

  const close = () => {
    visible.value = false;
  };

  const disposeChart = () => {
    if (chart) {
      chart.dispose();
      chart = null;
    }
    if (resizeObserver) {
      resizeObserver.disconnect();
      resizeObserver = null;
    }
  };

  watch(
    visible,
    async (newVal) => {
      if (newVal) {
        await nextTick();
        setTimeout(() => {
          if (visible.value && chartRef.value?.offsetWidth) {
            initChart();
          }
        }, 100);
      } else {
        disposeChart();
      }
    },
    { immediate: true }
  );

  watch(
    () => props.modelValue,
    (val) => (visible.value = val)
  );
  watch(visible, (val) => emit("update:modelValue", val));

  onBeforeUnmount(disposeChart);
</script>

<style lang="scss" scoped>
  .price-history {
    display: flex;
    flex-direction: column;
    justify-content: flex-start;
    background: var(--vt-c-dark-blue-gray);
    color: var(--vt-c-white);
    padding: 24px;
    border-radius: 12px;
    box-shadow: 0 10px 30px rgba(0, 0, 0, 0.3);
    max-height: 85vh;
    max-width: 80vw;
    width: min(640px, 90vw);

    &__overlay {
      position: fixed;
      inset: 0;
      background: rgba(0, 0, 0, 0.5);
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
      margin-bottom: 20px;
    }

    &__title {
      font-size: 20px;
      font-weight: 600;
      color: var(--vt-c-white);
      margin: 0;
    }

    &__close {
      background: none;
      border: none;
      font-size: 30px;
      color: inherit;
      font-weight: 500;
      transition: 0.2s;
      cursor: pointer;
      line-height: 0.8;
      display: flex;
      align-items: center;
      justify-content: center;
      padding: 0;
      margin: 0;
      width: 36px;
      height: 36px;

      @media (hover: hover) and (pointer: fine) {
        &:hover {
          color: var(--vt-c-light-red);
          transition: 0.2s;
        }
      }
    }

    &__body {
      display: flex;
      flex-direction: column;
      flex-grow: 1;
      min-height: 0;
      overflow: hidden;
    }

    &__content {
      display: flex;
      flex-direction: column;
      overflow: hidden;
      width: 100%;
      height: 100%;
    }

    &__chart {
      height: 450px;
      width: 100%;
      border-radius: 8px;
      overflow: hidden;

      @media (max-width: 768px) {
        height: 350px;
      }
    }
  }

  .overlay-enter-active,
  .overlay-leave-active {
    transition: opacity 0.3s ease;
  }

  .overlay-enter-from,
  .overlay-leave-to {
    opacity: 0;
  }

  .modal-enter-active,
  .modal-leave-active {
    transition: transform 0.3s ease;
  }

  .modal-enter-from,
  .modal-leave-to {
    transform: scale(0.95);
  }
</style>
