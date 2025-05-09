import { createRouter, createWebHistory } from 'vue-router';
import type { RouteRecordRaw } from 'vue-router';
import { useAuth } from '@/composables/useAuth';
import { useUsers } from '@/composables/useUsers';

const routes: RouteRecordRaw[] = [
  {
    path: '/login',
    name: 'AuthPage',
    component: () => import('@/pages/AuthPage.vue'),
    meta: { requiresAuth: false }
  },
  {
    path: '/',
    name: 'HomePage',
    component: () => import('@/pages/HomePage.vue'),
    meta: { requiresAuth: false }
  },
  {
    path: '/receipt',
    name: 'ReceiptPage',
    component: () => import('@/pages/ReceiptPage.vue'),
    meta: { requiresAuth: true }
  },
  {
    path: '/profile',
    name: 'ProfilePage',
    component: () => import('@/pages/ProfilePage.vue'),
    meta: { requiresAuth: true }
  },
  {
    path: '/contacts',
    name: 'ContactsPage',
    component: () => import('@/pages/ContactsPage.vue'),
    meta: { requiresAuth: true }
  },
  {
    path: '/help',
    name: 'HelpPage',
    component: () => import('@/pages/HelpPage.vue'),
    meta: { requiresAuth: true }
  },
  {
    path: '/admin',
    name: 'AdminPage',
    component: () => import('@/pages/AdminPage.vue'),
    meta: { requiresAuth: true, roles: ['Admin'] }
  },
  {
    path: '/:pathMatch(.*)*',
    redirect: '/'
  }
];

const router = createRouter({
  history: createWebHistory(),
  routes,
});

router.beforeEach(async (to, from, next) => {
  const auth = useAuth();
  const users = useUsers();

  if (to.meta.requiresAuth) {
    try {
      await auth.checkAuthStatus();
      await users.getUserInfo();
    } catch (error) {
      console.error('Ошибка при проверке аутентификации:', error);
    }
  }

  if (to.meta.requiresAuth && !auth.isAuthenticated.value) {
    next({ name: 'HomePage' });
    return;
  }

  if (to.name === 'AuthPage' && auth.isAuthenticated.value) {
    next({ name: 'HomePage' });
    return;
  }

  if (to.meta.roles && Array.isArray(to.meta.roles)) {
    if (!users.role.value || !to.meta.roles.includes(users.role.value)) {
      next({ name: 'HomePage' });
      return;
    }
  }

  next();
});

export default router;
