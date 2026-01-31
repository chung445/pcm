<template>
  <div>
    <h1 class="mb-4"><i class="bi bi-newspaper"></i> Tin tức</h1>

    <div class="mb-3">
      <label class="form-label">Lọc theo ghim</label>
      <select class="form-select" v-model="filterPinned" @change="fetchNews">
        <option :value="undefined">Tất cả</option>
        <option :value="true">Chỉ ghim</option>
        <option :value="false">Không ghim</option>
      </select>
    </div>

    <!-- Admin panel: Create / Edit -->
    <div v-if="isAdmin" class="card mb-3 p-3">
      <h5>{{ editing ? 'Chỉnh sửa tin' : 'Tạo tin mới' }}</h5>
      <div class="mb-2">
        <input v-model="form.title" class="form-control" placeholder="Tiêu đề" />
      </div>
      <div class="mb-2">
        <textarea v-model="form.content" class="form-control" rows="4" placeholder="Nội dung"></textarea>
      </div>
      <div class="form-check mb-2">
        <input class="form-check-input" type="checkbox" v-model="form.isPinned" id="pinCheck" />
        <label class="form-check-label" for="pinCheck">Ghim lên đầu</label>
      </div>
      <div>
        <button class="btn btn-primary me-2" @click="editing ? updateNews() : createNews()">{{ editing ? 'Cập nhật' : 'Tạo' }}</button>
        <button v-if="editing" class="btn btn-secondary me-2" @click="cancelEdit()">Hủy</button>
      </div>
    </div>

    <div v-if="loading" class="text-center"><div class="spinner-border"></div></div>

    <div v-else>
      <div v-for="item in news" :key="item.id" class="card mb-2">
        <div class="card-body">
          <h5 class="card-title d-flex justify-content-between align-items-start">
            <span>{{ item.title }}</span>
            <small>
              <span v-if="item.isPinned" class="badge bg-warning text-dark me-2">Ghim</span>
              <span class="text-muted">{{ new Date(item.createdDate).toLocaleString() }}</span>
            </small>
          </h5>
          <p class="card-text">{{ item.content }}</p>
          <div v-if="isAdmin" class="mt-2">
            <button class="btn btn-sm btn-outline-primary me-2" @click="startEdit(item)">Sửa</button>
            <button class="btn btn-sm btn-danger" @click="deleteNews(item.id)">Xóa</button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue'
import { newsAPI } from '@/services/api'
import { useAuthStore } from '@/stores/auth'

const auth = useAuthStore()
const isAdmin = computed(() => auth.isAdmin)

const news = ref([])
const loading = ref(false)
const filterPinned = ref(undefined)

const form = ref({ id: null, title: '', content: '', isPinned: false })
const editing = ref(false)

const fetchNews = async () => {
  loading.value = true
  try {
    const res = await newsAPI.getAll(filterPinned.value)
    news.value = res.data
  } catch (err) {
    console.error(err)
  } finally {
    loading.value = false
  }
}

const createNews = async () => {
  try {
    await newsAPI.create({ title: form.value.title, content: form.value.content, isPinned: form.value.isPinned })
    form.value.title = ''
    form.value.content = ''
    form.value.isPinned = false
    fetchNews()
    alert('Tạo tin thành công')
  } catch (err) {
    console.error(err)
    alert('Không thể tạo tin. Đăng nhập là Admin?')
  }
}

const startEdit = (item) => {
  editing.value = true
  form.value.id = item.id
  form.value.title = item.title
  form.value.content = item.content
  form.value.isPinned = item.isPinned
  window.scrollTo({ top: 0, behavior: 'smooth' })
}

const cancelEdit = () => {
  editing.value = false
  form.value.id = null
  form.value.title = ''
  form.value.content = ''
  form.value.isPinned = false
}

const updateNews = async () => {
  try {
    await newsAPI.update(form.value.id, { title: form.value.title, content: form.value.content, isPinned: form.value.isPinned })
    cancelEdit()
    fetchNews()
    alert('Cập nhật thành công')
  } catch (err) {
    console.error(err)
    alert('Không thể cập nhật')
  }
}

const deleteNews = async (id) => {
  if (!confirm('Bạn có chắc chắn muốn xóa tin này?')) return
  try {
    await newsAPI.delete(id)
    fetchNews()
    alert('Đã xóa')
  } catch (err) {
    console.error(err)
    alert('Không thể xóa')
  }
}

onMounted(fetchNews)
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
