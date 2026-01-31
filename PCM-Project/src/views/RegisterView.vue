<template>
  <div class="register-container">
    <div class="row justify-content-center">
      <div class="col-md-6">
        <div class="card shadow">
          <div class="card-body p-5">
            <div class="text-center mb-4">
              <h2 class="fw-bold text-primary">Đăng ký tài khoản</h2>
              <p class="text-muted">Tham gia CLB Pickleball "Vợt Thủ Phố Núi"</p>
            </div>

            <form @submit.prevent="handleRegister">
              <div class="mb-3">
                <label class="form-label">Họ và tên</label>
                <input type="text" class="form-control" v-model="formData.fullName" required />
              </div>

              <div class="mb-3">
                <label class="form-label">Email</label>
                <input type="email" class="form-control" v-model="formData.email" required />
              </div>

              <div class="mb-3">
                <label class="form-label">Số điện thoại</label>
                <input type="tel" class="form-control" v-model="formData.phoneNumber" />
              </div>

              <div class="mb-3">
                <label class="form-label">Ngày sinh</label>
                <input type="date" class="form-control" v-model="formData.dateOfBirth" />
              </div>

              <div class="mb-3">
                <label class="form-label">Mật khẩu</label>
                <input type="password" class="form-control" v-model="formData.password" required minlength="6" />
              </div>

              <div v-if="error" class="alert alert-danger">{{ error }}</div>

              <button type="submit" class="btn btn-primary btn-lg w-100 mb-3" :disabled="loading">
                <span v-if="loading" class="spinner-border spinner-border-sm me-2"></span>
                Đăng ký
              </button>

              <div class="text-center">
                <p>Đã có tài khoản? <router-link to="/login">Đăng nhập</router-link></p>
              </div>
            </form>
          </div>
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

const formData = ref({
  fullName: '',
  email: '',
  phoneNumber: '',
  dateOfBirth: '',
  password: ''
})

const loading = ref(false)
const error = ref('')

const handleRegister = async () => {
  try {
    loading.value = true
    error.value = ''

    await authStore.register(formData.value)
    
    toast.success('Đăng ký thành công!')
    router.push('/dashboard')
  } catch (err) {
    error.value = 'Đăng ký thất bại. Email có thể đã tồn tại.'
    toast.error('Đăng ký thất bại!')
  } finally {
    loading.value = false
  }
}
</script>

<style scoped>
.register-container {
  min-height: 100vh;
  display: flex;
  align-items: center;
  background: linear-gradient(135deg, #10b981 0%, #059669 100%);
  padding: 2rem 0;
}
</style>
