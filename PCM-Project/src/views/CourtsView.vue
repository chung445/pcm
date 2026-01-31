<template>
  <div>
    <h1 class="mb-4"><i class="bi bi-geo-alt"></i> Danh sách Sân</h1>

    <!-- Admin: create / edit court -->
    <div v-if="isAdmin" class="card mb-3 p-3">
      <h5 class="mb-2">{{ editing ? 'Chỉnh sửa sân' : 'Thêm sân mới' }}</h5>
      <div class="row g-2">
        <div class="col-md-4">
          <input v-model="form.name" class="form-control" placeholder="Tên sân" />
        </div>
        <div class="col-md-4">
          <div class="form-check">
            <input class="form-check-input" type="checkbox" v-model="form.isActive" id="courtActive" />
            <label class="form-check-label" for="courtActive">Hoạt động</label>
          </div>
        </div>
        <div class="col-md-4 d-flex align-items-end">
          <button class="btn btn-primary me-2" @click="editing ? updateCourt() : createCourt()">{{ editing ? 'Cập nhật' : 'Tạo' }}</button>
          <button v-if="editing" class="btn btn-secondary" @click="cancelEdit()">Hủy</button>
        </div>
      </div>
      <div class="mt-2">
        <textarea v-model="form.description" class="form-control" rows="2" placeholder="Mô tả"></textarea>
      </div>
    </div>

    <div v-if="loading" class="text-center"><div class="spinner-border"></div></div>

    <div v-else>
      <div class="list-group">
        <div v-for="c in courts" :key="c.id" class="list-group-item d-flex justify-content-between align-items-start">
          <div>
            <h5 class="mb-1">{{ c.name }} <small class="text-muted">• {{ c.isActive ? 'Hoạt động' : 'Tạm dừng' }}</small></h5>
            <p class="mb-0">{{ c.description }}</p>
          </div>
          <div v-if="isAdmin">
            <button class="btn btn-sm btn-outline-primary me-2" @click="startEdit(c)">Sửa</button>
            <button class="btn btn-sm btn-danger" @click="deleteCourt(c.id)">Xóa</button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue'
import { courtsAPI } from '@/services/api'
import { useAuthStore } from '@/stores/auth'

const auth = useAuthStore()
const isAdmin = computed(() => auth.isAdmin)

const courts = ref([])
const loading = ref(false)
const editing = ref(false)
const form = ref({ id: null, name: '', description: '', isActive: true })

const fetchCourts = async () => {
  loading.value = true
  try {
    const res = await courtsAPI.getAll()
    courts.value = res.data
  } catch (err) {
    console.error(err)
  } finally {
    loading.value = false
  }
}

const createCourt = async () => {
  try {
    await courtsAPI.create({ name: form.value.name, description: form.value.description, isActive: form.value.isActive })
    form.value.name = ''
    form.value.description = ''
    form.value.isActive = true
    fetchCourts()
    alert('Tạo sân thành công')
  } catch (err) {
    console.error(err)
    alert('Không thể tạo sân. Bạn có phải Admin?')
  }
}

const startEdit = (c) => {
  editing.value = true
  form.value.id = c.id
  form.value.name = c.name
  form.value.description = c.description
  form.value.isActive = c.isActive
  window.scrollTo({ top: 0, behavior: 'smooth' })
}

const cancelEdit = () => {
  editing.value = false
  form.value = { id: null, name: '', description: '', isActive: true }
}

const updateCourt = async () => {
  try {
    await courtsAPI.update(form.value.id, { name: form.value.name, description: form.value.description, isActive: form.value.isActive })
    cancelEdit()
    fetchCourts()
    alert('Cập nhật thành công')
  } catch (err) {
    console.error(err)
    alert('Không thể cập nhật sân')
  }
}

const deleteCourt = async (id) => {
  if (!confirm('Bạn có chắc chắn muốn xóa sân này?')) return
  try {
    await courtsAPI.delete(id)
    fetchCourts()
    alert('Đã xóa sân')
  } catch (err) {
    console.error(err)
    alert('Không thể xóa sân')
  }
}

onMounted(fetchCourts)
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
