<template>
  <div class="login-bg">
    <div class="login-center">
      <div class="login-card">
        <div class="text-center mb-4">
          <h2 class="fw-bold text-primary">
            <i class="bi bi-trophy-fill"></i> PCM Club
          </h2>
          <p class="text-muted">Đăng nhập vào hệ thống</p>
        </div>
        <form @submit.prevent="handleLogin">
          <div class="mb-3">
            <label for="email" class="form-label">Email</label>
            <input
              type="email"
              class="form-control form-control-lg"
              id="email"
              v-model="credentials.email"
              required
              placeholder="email@example.com"
            />
          </div>
          <div class="mb-3">
            <label for="password" class="form-label">Mật khẩu</label>
            <input
              type="password"
              class="form-control form-control-lg"
              id="password"
              v-model="credentials.password"
              required
              placeholder="••••••••"
            />
          </div>
          <div v-if="error" class="alert alert-danger">
            {{ error }}
          </div>
          <button type="submit" class="btn btn-primary btn-lg w-100 mb-3" :disabled="loading">
            <span v-if="loading" class="spinner-border spinner-border-sm me-2"></span>
            Đăng nhập
          </button>
          <div class="text-center">
            <p class="mb-0">
              Chưa có tài khoản? 
              <router-link to="/register" class="text-primary fw-bold">Đăng ký ngay</router-link>
            </p>
          </div>
        </form>
        <hr class="my-4">
        <div class="alert alert-info">
          <strong>Tài khoản demo:</strong><br>
          Admin: admin@pcm.com / Admin@123<br>
          Member: an@pcm.com / Member@123
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth'
import { useToast } from 'vue-toastification'

const router = useRouter()
const authStore = useAuthStore()
const toast = useToast()

const credentials = ref({
  email: '',
  password: ''
})

const loading = ref(false)
const error = ref('')

const handleLogin = async () => {
  try {
    loading.value = true
    error.value = ''

    await authStore.login(credentials.value)
    
    toast.success('Đăng nhập thành công!')
    router.push('/dashboard')
  } catch (err) {
    error.value = 'Email hoặc mật khẩu không đúng'
    toast.error('Đăng nhập thất bại!')
  } finally {
    loading.value = false
  }
}
</script>

<style scoped>
.login-bg {
  min-height: 100vh;
  width: 100vw;
  display: flex;
  align-items: center;
  justify-content: center;
  background: linear-gradient(135deg, #10b981 0%, #059669 100%);
}
.login-center {
  width: 100vw;
  display: flex;
  align-items: center;
  justify-content: center;
}
.login-card {
  background: #fff;
  border-radius: 1.5rem;
  box-shadow: 0 4px 32px rgba(0,0,0,0.08);
  padding: 2.5rem 2rem;
  min-width: 340px;
  max-width: 380px;
  width: 100%;
}
</style>
