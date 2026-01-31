<template>
  <div id="app">
    <nav v-if="!['Login','Register'].includes($route.name)" class="navbar navbar-expand-lg navbar-light bg-white shadow-sm">
      <div class="container-fluid">
        <router-link to="/" class="navbar-brand">
          <i class="bi bi-trophy"></i> PCM Club
        </router-link>
        <button class="navbar-toggler" type="button" data-bs-toggle="collapse" data-bs-target="#navbarNav">
          <span class="navbar-toggler-icon"></span>
        </button>
        <div class="collapse navbar-collapse" id="navbarNav">
          <ul class="navbar-nav me-auto">
            <li class="nav-item">
              <router-link to="/dashboard" class="nav-link">
                <i class="bi bi-speedometer2"></i> Dashboard
              </router-link>
            </li>
            <li class="nav-item">
              <router-link to="/members" class="nav-link">
                <i class="bi bi-people"></i> Hội viên
              </router-link>
            </li>
            <li class="nav-item">
              <router-link to="/news" class="nav-link">
                <i class="bi bi-newspaper"></i> Tin tức
              </router-link>
            </li>
            <li class="nav-item">
              <router-link to="/bookings" class="nav-link">
                <i class="bi bi-calendar-check"></i> Đặt sân
              </router-link>
            </li>
            <li class="nav-item">
              <router-link to="/challenges" class="nav-link">
                <i class="bi bi-trophy-fill"></i> Sàn đấu
              </router-link>
            </li>
            <li class="nav-item">
              <router-link to="/matches" class="nav-link">
                <i class="bi bi-clipboard-data"></i> Trận đấu
              </router-link>
            </li>
            <li class="nav-item" v-if="authStore.isAdmin || authStore.isTreasurer">
              <router-link to="/transactions" class="nav-link">
                <i class="bi bi-wallet2"></i> Tài chính
              </router-link>
            </li>
            <li class="nav-item" v-if="authStore.isAdmin">
              <router-link to="/courts" class="nav-link">
                <i class="bi bi-pin-map"></i> Sân
              </router-link>
            </li>
          </ul>
          <ul class="navbar-nav">
            <li class="nav-item dropdown">
              <a class="nav-link dropdown-toggle" href="#" role="button" data-bs-toggle="dropdown">
                <i class="bi bi-person-circle"></i> {{ authStore.userName }}
              </a>
              <ul class="dropdown-menu dropdown-menu-end">
                <li><a class="dropdown-item" href="#"><i class="bi bi-person"></i> Hồ sơ</a></li>
                <li><hr class="dropdown-divider"></li>
                <li><a class="dropdown-item" href="#" @click.prevent="handleLogout">
                  <i class="bi bi-box-arrow-right"></i> Đăng xuất
                </a></li>
              </ul>
            </li>
          </ul>
        </div>
      </div>
    </nav>

    <main class="container-fluid py-4">
      <router-view />
    </main>

    <footer v-if="authStore.isAuthenticated && !['Login','Register'].includes($route.name)" class="text-center py-3 mt-5 bg-light">
      <div class="container">
        <p class="mb-0 text-muted">
          &copy; 2026 CLB Pickleball "Vợt Thủ Phố Núi" - Vui • Khỏe • Có Thưởng
        </p>
      </div>
    </footer>
  </div>
</template>

<script setup>
import { useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth'

const router = useRouter()
const authStore = useAuthStore()

const handleLogout = () => {
  authStore.logout()
  router.push('/login')
}
</script>

<style scoped>
.navbar {
  margin-bottom: 0;
}

.nav-link {
  font-weight: 500;
  transition: color 0.2s;
}

.nav-link:hover {
  color: var(--primary-color) !important;
}

.router-link-active {
  color: var(--primary-color) !important;
}
</style>

<style>
body {
  font-family: 'Montserrat', 'Roboto', Arial, sans-serif;
  background: #f6f8fa;
  color: #222;
}
.card, .login-card {
  border-radius: 1.25rem;
  box-shadow: 0 4px 24px rgba(0,0,0,0.08);
  border: none;
}
.btn, .form-control, .form-select {
  border-radius: 0.75rem;
  font-size: 1rem;
}
.navbar {
  border-radius: 0 0 1.25rem 1.25rem;
  box-shadow: 0 2px 12px rgba(0,0,0,0.06);
}
.stat-card {
  border-radius: 1rem;
  box-shadow: 0 2px 12px rgba(0,0,0,0.07);
  font-weight: 600;
}
.badge {
  font-size: 1rem;
  border-radius: 0.5rem;
  padding: 0.5em 1em;
}
.list-group-item {
  border-radius: 0.75rem !important;
  margin-bottom: 0.5rem;
  box-shadow: 0 2px 8px rgba(0,0,0,0.04);
}
input, textarea, select {
  border-radius: 0.75rem !important;
}
footer {
  border-radius: 1.25rem 1.25rem 0 0;
  box-shadow: 0 -2px 12px rgba(0,0,0,0.06);
}
@media (max-width: 768px) {
  .card, .login-card, .stat-card, .navbar, footer {
    border-radius: 0.75rem;
  }
}
</style>
