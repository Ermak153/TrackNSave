import { createRouter, createWebHistory} from 'vue-router';
import type { RouteRecordRaw } from 'vue-router';

const routes: RouteRecordRaw[] = [
  {
    path: '/login',
    name: 'AuthPage',
    component: () => import('@/pages/AuthPage.vue'),
  },
  {
    path: '/',
    name: 'HomePage',
    component: () => import('@/pages/HomePage.vue'),
  },
  {
    path: '/receipt',
    name: 'ReceiptPage',
    component: () => import('@/pages/ReceiptPage.vue')
  },
  {
    path: '/profile',
    name: 'ProfilePage',
    component: () => import('@/pages/ProfilePage.vue')
  },
  {
    path: '/contacts',
    name: 'ContactsPage',
    component: () => import('@/pages/ContactsPage.vue')
  },
  {
    path: '/help',
    name: 'HelpPage',
    component: () => import('@/pages/HelpPage.vue')
  }
];

const router = createRouter({
  history: createWebHistory(),
  routes,
});

export default router;

