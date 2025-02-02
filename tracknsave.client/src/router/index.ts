import { createRouter, createWebHistory} from 'vue-router';
import type { RouteRecordRaw } from 'vue-router';

const routes: RouteRecordRaw[] = [
  {
    path: '/',
    name: 'HomePage',
    component: () => import('@/pages/HomePage.vue'),
  },
  {
    path: '/login',
    name: 'AuthPage',
    component: () => import('@/pages/AuthPage.vue'),
  },
];

const router = createRouter({
  history: createWebHistory(),
  routes,
});

export default router;

