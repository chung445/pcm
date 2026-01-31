import { createRouter, createWebHistory } from 'vue-router'
import { useAuthStore } from '@/stores/auth'

const routes = [
  {
    path: '/',
    redirect: '/dashboard'
  },
  {
    path: '/login',
    name: 'Login',
    component: () => import('@/views/LoginView.vue'),
    meta: { requiresAuth: false }
  },
  {
    path: '/register',
    name: 'Register',
    component: () => import('@/views/RegisterView.vue'),
    meta: { requiresAuth: false }
  },
  {
    path: '/dashboard',
    name: 'Dashboard',
    component: () => import('@/views/DashboardView.vue'),
    meta: { requiresAuth: true }
  },
  {
    path: '/members',
    name: 'Members',
    component: () => import('@/views/MembersView.vue'),
    meta: { requiresAuth: false }
  },
  {
    path: '/members/:id',
    name: 'MemberDetail',
    component: () => import('@/views/MemberDetailView.vue'),
    meta: { requiresAuth: true }
  },
  {
    path: '/news',
    name: 'News',
    component: () => import('@/views/NewsView.vue'),
    meta: { requiresAuth: false }
  },
  {
    path: '/courts',
    name: 'Courts',
    component: () => import('@/views/CourtsView.vue'),
    meta: { requiresAuth: false }
  },
  {
    path: '/bookings',
    name: 'Bookings',
    component: () => import('@/views/BookingsView.vue'),
    meta: { requiresAuth: false }
  },
  {
    path: '/challenges',
    name: 'Challenges',
    component: () => import('@/views/ChallengesView.vue'),
    meta: { requiresAuth: false }
  },
  {
    path: '/matches',
    name: 'Matches',
    component: () => import('@/views/MatchesView.vue'),
    meta: { requiresAuth: false }
  },
  {
    path: '/transactions',
    name: 'Transactions',
    component: () => import('@/views/TransactionsView.vue'),
    meta: { requiresAuth: false }
  }
]

const router = createRouter({
  history: createWebHistory(),
  routes
})

router.beforeEach((to, from, next) => {
  const authStore = useAuthStore()
  const requiresAuth = to.matched.some(record => record.meta.requiresAuth)

  if (requiresAuth && !authStore.isAuthenticated) {
    next('/login')
  } else if (!requiresAuth && authStore.isAuthenticated && (to.name === 'Login' || to.name === 'Register')) {
    next('/dashboard')
  } else {
    next()
  }
})

export default router
