<template>
  <div>
    <h1 class="mb-4"><i class="bi bi-people-fill"></i> Sàn đấu / Challenges</h1>

    <div v-if="loading" class="text-center"><div class="spinner-border"></div></div>

    <div v-else>
      <div v-if="isAdmin" class="card mb-3 p-3">
        <h5>{{ editing ? 'Chỉnh sửa Challenge' : 'Tạo Challenge mới' }}</h5>
        <div class="mb-2">
          <input v-model="form.title" class="form-control" placeholder="Tiêu đề" />
        </div>
        <div class="mb-2">
          <textarea v-model="form.description" class="form-control" rows="2" placeholder="Mô tả"></textarea>
        </div>
        <div class="mb-2">
          <input v-model="form.startTime" type="datetime-local" class="form-control" placeholder="Bắt đầu" />
        </div>
        <div class="mb-2">
          <input v-model="form.endTime" type="datetime-local" class="form-control" placeholder="Kết thúc" />
        </div>
        <div>
          <button class="btn btn-primary me-2" @click="editing ? updateChallenge() : createChallenge()">{{ editing ? 'Cập nhật' : 'Tạo' }}</button>
          <button v-if="editing" class="btn btn-secondary me-2" @click="cancelEdit()">Hủy</button>
        </div>
      </div>

      <ul class="list-group">
        <li v-for="c in challenges" :key="c.id" class="list-group-item d-flex justify-content-between align-items-center">
          <div>
            <strong>{{ c.title }}</strong>
            <div>{{ c.description }}</div>
            <small>Thời gian: {{ new Date(c.startTime).toLocaleString() }}</small>
          </div>
          <div v-if="isAdmin">
            <button class="btn btn-sm btn-outline-primary me-2" @click="startEdit(c)">Sửa</button>
            <button class="btn btn-sm btn-danger" @click="deleteChallenge(c.id)">Xóa</button>
          </div>
        </li>
      </ul>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue'
import { challengesAPI } from '@/services/api'
import { useAuthStore } from '@/stores/auth'

const auth = useAuthStore()
const isAdmin = computed(() => auth.isAdmin)

const challenges = ref([])
const loading = ref(false)
const editing = ref(false)
const form = ref({ id: null, title: '', description: '', startTime: '', endTime: '' })

const fetchChallenges = async () => {
  loading.value = true
  try {
    const res = await challengesAPI.getAll()
    challenges.value = res.data
  } catch (err) {
    console.error(err)
  } finally {
    loading.value = false
  }
}

const createChallenge = async () => {
  try {
    await challengesAPI.create({
      title: form.value.title,
      description: form.value.description,
      startTime: new Date(form.value.startTime).toISOString(),
      endTime: new Date(form.value.endTime).toISOString()
    })
    form.value = { id: null, title: '', description: '', startTime: '', endTime: '' }
    fetchChallenges()
    alert('Tạo challenge thành công')
  } catch (err) {
    console.error(err)
    alert('Không thể tạo challenge')
  }
}

const startEdit = (c) => {
  editing.value = true
  form.value.id = c.id
  form.value.title = c.title
  form.value.description = c.description
  form.value.startTime = c.startTime?.slice(0,16)
  form.value.endTime = c.endTime?.slice(0,16)
}

const cancelEdit = () => {
  editing.value = false
  form.value = { id: null, title: '', description: '', startTime: '', endTime: '' }
}

const updateChallenge = async () => {
  try {
    await challengesAPI.update(form.value.id, {
      title: form.value.title,
      description: form.value.description,
      startTime: new Date(form.value.startTime).toISOString(),
      endTime: new Date(form.value.endTime).toISOString()
    })
    cancelEdit()
    fetchChallenges()
    alert('Cập nhật thành công')
  } catch (err) {
    console.error(err)
    alert('Không thể cập nhật challenge')
  }
}

const deleteChallenge = async (id) => {
  if (!confirm('Bạn có chắc chắn muốn xóa challenge này?')) return
  try {
    await challengesAPI.delete(id)
    fetchChallenges()
    alert('Đã xóa challenge')
  } catch (err) {
    console.error(err)
    alert('Không thể xóa challenge')
  }
}

onMounted(fetchChallenges)
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