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
  {
    path: '/profile',
    name: 'ProfilePage',
    component: () => import('@/pages/ProfilePage.vue')
  },
  {
    path: '/receipt',
    name: 'ReceiptPage',
    component: () => import('@/pages/ReceiptPage.vue')
  }
];

const router = createRouter({
  history: createWebHistory(),
  routes,
});

export default router;

