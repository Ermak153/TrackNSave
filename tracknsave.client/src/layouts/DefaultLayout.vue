<template>
  <div class="default-layout">
    <HeaderTNS class="header"></HeaderTNS>
    <div class="content">
      <slot name="default">
        <RouterView class="default" />
      </slot>
    </div>
    <FooterTNS class="footer"></FooterTNS>
  </div>
</template>

<script setup lang="ts">
  import HeaderTNS from "@/components/HeaderTNS.vue";
  import FooterTNS from "@/components/FooterTNS.vue";
</script>

<style lang="scss" scoped>
  .default-layout {
    position: relative;
    min-height: 100vh;
    display: flex;
    flex-direction: column;
    @media (max-width: 768px) {
      min-height: 90vh;
    }
  }

  .content {
    padding-top: 150px;
    flex: 1;
    display: flex;
    flex-direction: column;

    @media (max-width: 768px) {
      padding-top: 100px;
    }
  }
</style>
