<template>
  <div style="display: flex; justify-content: center;">
    <ReceiptCard :receipt="testReceipt"/>
  </div>
</template>

<script setup lang="ts">
  import ReceiptCard from '@/components/ReceiptCard.vue';

  const testReceipt = {
    user: "ООО \"ДельтаТорг\"",
    totalSum: 61449,
    dateTime: "2024-12-24T14:37:00",
    retailPlace: "Магазин Быстроном",
    items: [
      {
        name: "Сок САДЫ АЛТАЯ виноград пэт 2л",
        price: 12797,
        quantity: 1,
        sum: 12797
      },
      {
        name: "Молоко БЕЛЫЙ ЗАМОК цельное отборное бзмж 3.8% т/пак 900г",
        price: 10998,
        quantity: 1,
        sum: 10998
      },
      {
        name: "Рожок ВОСХОД сдобный 1с 60г",
        price: 799,
        quantity: 4,
        sum: 3196
      }
    ]
  }
</script>

<style lang="scss" scoped>

</style>
