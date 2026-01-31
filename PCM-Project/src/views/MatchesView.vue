<template>
  <div>
    <h1 class="mb-4"><i class="bi bi-trophy"></i> Trận đấu</h1>

    <div v-if="loading" class="text-center"><div class="spinner-border"></div></div>

    <div v-else>
      <ul class="list-group">
        <li v-for="m in matches" :key="m.id" class="list-group-item">
          <strong>{{ m.title || ('Match #' + m.id) }}</strong>
          <div>{{ new Date(m.scheduledAt).toLocaleString() }} • Sân: {{ m.courtName }}</div>
        </li>
      </ul>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { matchesAPI } from '@/services/api'

const matches = ref([])
const loading = ref(false)

const fetchMatches = async () => {
  loading.value = true
  try {
    const res = await matchesAPI.getAll()
    matches.value = res.data
  } catch (err) {
    console.error(err)
  } finally {
    loading.value = false
  }
}

onMounted(fetchMatches)
</script>

<style scoped>
.card {
  border-radius: 1.25rem;
  box-shadow: 0 4px 24px rgba(0,0,0,0.08);
  border: none;
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