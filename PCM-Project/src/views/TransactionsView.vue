<template>
  <div>
    <h1 class="mb-4"><i class="bi bi-wallet2"></i> Tài chính</h1>

    <div v-if="loading" class="text-center"><div class="spinner-border"></div></div>

    <div v-else>
      <div class="row">
        <div class="col-md-4">
          <div class="card mb-3">
            <div class="card-body">
              <h5 class="card-title">Tổng thu</h5>
              <p class="card-text">{{ summary.totalIncome | currency }}</p>
            </div>
          </div>
        </div>
        <div class="col-md-4">
          <div class="card mb-3">
            <div class="card-body">
              <h5 class="card-title">Tổng chi</h5>
              <p class="card-text">{{ summary.totalExpense | currency }}</p>
            </div>
          </div>
        </div>
      </div>

      <h5 class="mt-3">Giao dịch gần đây</h5>
      <ul class="list-group">
        <li v-for="t in transactions" :key="t.id" class="list-group-item d-flex justify-content-between align-items-center">
          <div>
            <strong>{{ t.description }}</strong>
            <div class="text-muted small">{{ t.date }} • {{ t.amount | currency }} • {{ t.categoryName }} • <span class="badge bg-secondary">{{ t.status }}</span></div>
          </div>
          <div>
            <button v-if="(auth.isAdmin || auth.isTreasurer) && t.status === 'Pending'" class="btn btn-success btn-sm me-1" @click="approve(t.id)">Duyệt</button>
            <button v-if="(auth.isAdmin || auth.isTreasurer) && t.status === 'Pending'" class="btn btn-danger btn-sm" @click="reject(t.id)">Từ chối</button>
          </div>
        </li>
      </ul>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { transactionsAPI } from '@/services/api'
import { useAuthStore } from '@/stores/auth'

const transactions = ref([])
const summary = ref({ totalIncome: 0, totalExpense: 0 })
const loading = ref(false)
const auth = useAuthStore()

const loadData = async () => {
  loading.value = true
  try {
    const res = await transactionsAPI.getAll()
    transactions.value = res.data
    const s = await transactionsAPI.getSummary()
    summary.value = s.data
  } catch (err) {
    console.error(err)
  } finally {
    loading.value = false
  }
}

onMounted(loadData)

const approve = async (id) => {
  try {
    await transactionsAPI.approve(id)
    await loadData()
  } catch (err) {
    console.error(err)
    alert('Không thể duyệt giao dịch.')
  }
}

const reject = async (id) => {
  try {
    await transactionsAPI.reject(id)
    await loadData()
  } catch (err) {
    console.error(err)
    alert('Không thể từ chối giao dịch.')
  }
}

// Simple currency filter
const currency = (val) => {
  if (!val) return '0'
  return new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(val)
}

// expose filter
</script>

<script>
export default {
  filters: {
    currency: (v) => new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(v)
  }
}
</script>

<style scoped>
.card {
  border-radius: 1.25rem;
  box-shadow: 0 4px 24px rgba(0,0,0,0.08);
  border: none;
}
.form-control, .form-select {
  border-radius: 0.75rem;
  font-size: 1rem;
}
.btn {
  border-radius: 0.75rem;
  font-size: 1rem;
}
.list-group-item {
  border-radius: 0.75rem !important;
  margin-bottom: 0.5rem;
  box-shadow: 0 2px 8px rgba(0,0,0,0.04);
}
@media (max-width: 768px) {
  .card {
    border-radius: 0.75rem;
  }
}
</style>