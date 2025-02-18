<template>
  <footer class="footer">
    <div class="footer__container">
      <p class="footer__copyright">
        <span>© 2025, Track&Save. Все права защищены.</span>
        <span>Проект разработан в рамках дипломной работы</span>
      </p>
      <ul class="footer__logos">
        <li v-for="logo in logos" :key="logo.alt" class="footer__logo-item">
          <a
            :href="logo.link"
            target="_blank"
            rel="noopener noreferrer"
            class="footer__logo-link"
          >
            <img :src="logo.src" :alt="logo.alt" class="footer__logo-img" />
          </a>
        </li>
      </ul>
    </div>
  </footer>
</template>

<script setup lang="ts">
  const logos = [
    {
      src: "src/assets/icons/github.svg",
      alt: "Github",
      link: "https://github.com/Ermak153/TrackNSave",
    },
    {
      src: "src/assets/icons/telegram.svg",
      alt: "Telegram",
      link: "https://t.me/neko153",
    },
  ];
</script>

<style lang="scss" scoped>
  .footer {
    background-color: transparent;
    width: 100%;
    height: 115px;
    color: var(--vt-c-light-gray);
    text-align: center;

    &__container {
      max-width: 1280px;
      margin: 0 auto;
      padding: 10px;
      display: flex;
      flex-direction: row;
      align-items: end;
      justify-content: space-between;
    }

    &__logos {
      list-style: none;
      display: flex;
      gap: 20px;
      padding: 0;
    }

    &__logo-item {
      display: inline-block;
    }

    &__logo-link {
      display: inline-block;
      text-decoration: none;
      opacity: 0.6;
      transition: 0.2s;

      @media (hover: hover) and (pointer: fine) {
        &:hover {
          opacity: 1;
          transition: 0.2s;
        }
      }
    }

    &__logo-img {
      height: 40px;
      width: 40px;
    }

    &__copyright {
      font-size: 14px;

      span {
        display: block;
      }
    }
  }

  @media (max-width: 768px) {
    .footer {
      &__container {
        flex-direction: column;
        justify-content: center;
        align-items: center;
      }
    }

    .footer__logos {
      gap: 30px;

      &__logo-img {
        height: 30px;
        width: 30px;
      }
    }

    .footer__copyright {
      font-size: 12px;
    }
  }
</style>
