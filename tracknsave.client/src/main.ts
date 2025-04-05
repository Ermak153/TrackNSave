import './assets/main.css'

import { createApp } from 'vue'
import App from './App.vue'
import router from './router';
import ElementPlus from 'element-plus'
import 'element-plus/dist/index.css'
import { ru } from 'element-plus/es/locales.mjs';

createApp(App)
  .use(router)
  .use(ElementPlus, {locale: ru,})
  .mount('#app')
