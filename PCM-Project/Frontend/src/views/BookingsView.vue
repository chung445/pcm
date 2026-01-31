<template>
  <div>
    <h1 class="mb-4"><i class="bi bi-calendar-check"></i> Đặt sân</h1>

    <div class="card mb-3 p-3">
      <div class="row g-2">
        <div class="col-md-4">
          <label class="form-label">Chọn sân</label>
          <select class="form-select" v-model="form.courtId">
            <option v-for="c in courts" :key="c.id" :value="c.id">{{ c.name }}</option>
          </select>
        </div>
        <div class="col-md-3">
          <label class="form-label">Bắt đầu</label>
          <input type="datetime-local" class="form-control" v-model="form.startTime" />
        </div>
        <div class="col-md-3">
          <label class="form-label">Kết thúc</label>
          <input type="datetime-local" class="form-control" v-model="form.endTime" />
        </div>
        <div class="col-md-2 d-flex align-items-end">
          <button class="btn btn-primary w-100" @click="checkAvailability">Kiểm tra</button>
        </div>
      </div>

      <div v-if="available !== null" class="mt-2">
        <div v-if="available" class="alert alert-success">Khung giờ khả dụng. Bạn có thể đặt.</div>
        <div v-else class="alert alert-warning">Không khả dụng.</div>
        <button class="btn btn-success" :disabled="!available" @click="createBooking">Đặt ngay</button>
      </div>
    </div>

    <h4>Đặt trước</h4>
    <div v-if="loading" class="text-center"><div class="spinner-border"></div></div>
    <div v-else>
      <div v-if="isAdmin" class="mb-4">
        <h5>Lời đặt chờ duyệt</h5>
        <ul class="list-group mb-3">
          <li v-for="b in pendingBookings" :key="b.id" class="list-group-item d-flex justify-content-between align-items-start">
            <div>
              <strong>{{ b.courtName }}</strong> • {{ new Date(b.startTime).toLocaleString() }} - {{ new Date(b.endTime).toLocaleString() }}
              <div><small>{{ b.memberName }}</small></div>
            </div>
            <div>
              <button class="btn btn-sm btn-success me-1" @click="approve(b.id)">Duyệt</button>
              <button class="btn btn-sm btn-danger" @click="reject(b.id)">Từ chối</button>
            </div>
          </li>
        </ul>
      </div>

      <ul class="list-group">
        <li v-for="b in bookings" :key="b.id" class="list-group-item d-flex justify-content-between align-items-center">
          <div>
            <strong>{{ b.courtName }}</strong> • {{ new Date(b.startTime).toLocaleString() }} - {{ new Date(b.endTime).toLocaleString() }}
            <div><small v-if="b.status">Trạng thái: {{ b.status }}</small></div>
          </div>
          <div v-if="auth.user?.memberId === b.memberId && b.status === 'Pending'">
            <button class="btn btn-sm btn-outline-primary me-2" @click="startEdit(b)">Sửa</button>
            <button class="btn btn-sm btn-danger" @click="deleteBooking(b.id)">Xóa</button>
          </div>
        </li>
      </ul>

      <!-- Form sửa booking -->
      <div v-if="editing" class="card mt-3 p-3">
        <h5>Chỉnh sửa đặt sân</h5>
        <div class="row g-2">
          <div class="col-md-4">
            <label class="form-label">Chọn sân</label>
            <select class="form-select" v-model="editForm.courtId">
              <option v-for="c in courts" :key="c.id" :value="c.id">{{ c.name }}</option>
            </select>
          </div>
          <div class="col-md-3">
            <label class="form-label">Bắt đầu</label>
            <input type="datetime-local" class="form-control" v-model="editForm.startTime" />
          </div>
          <div class="col-md-3">
            <label class="form-label">Kết thúc</label>
            <input type="datetime-local" class="form-control" v-model="editForm.endTime" />
          </div>
          <div class="col-md-2 d-flex align-items-end">
            <button class="btn btn-primary w-100" @click="updateBooking">Cập nhật</button>
            <button class="btn btn-secondary w-100 ms-2" @click="cancelEdit">Hủy</button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue'
import { bookingsAPI, courtsAPI } from '@/services/api'
import { useAuthStore } from '@/stores/auth'

const auth = useAuthStore()
const isAdmin = computed(() => auth.isAdmin)

const bookings = ref([])
const courts = ref([])
const loading = ref(false)
const available = ref(null)

const form = ref({ courtId: null, startTime: '', endTime: '' })
const editing = ref(false)
const editForm = ref({ id: null, courtId: null, startTime: '', endTime: '' })
const startEdit = (b) => {
  editing.value = true
  editForm.value.id = b.id
  editForm.value.courtId = b.courtId
  editForm.value.startTime = b.startTime?.slice(0,16)
  editForm.value.endTime = b.endTime?.slice(0,16)
}

const cancelEdit = () => {
  editing.value = false
  editForm.value = { id: null, courtId: null, startTime: '', endTime: '' }
}

const updateBooking = async () => {
  try {
    await bookingsAPI.update(editForm.value.id, {
      courtId: editForm.value.courtId,
      startTime: new Date(editForm.value.startTime).toISOString(),
      endTime: new Date(editForm.value.endTime).toISOString()
    })
    cancelEdit()
    fetchData()
    alert('Cập nhật thành công')
  } catch (err) {
    console.error(err)
    alert('Không thể cập nhật')
  }
}

const deleteBooking = async (id) => {
  if (!confirm('Bạn có chắc chắn muốn xóa đặt sân này?')) return
  try {
    await bookingsAPI.delete(id)
    fetchData()
    alert('Đã xóa đặt sân')
  } catch (err) {
    console.error(err)
    alert('Không thể xóa')
  }
}

const fetchData = async () => {
  loading.value = true
  try {
    const c = await courtsAPI.getAll()
    courts.value = c.data
    if (courts.value.length) form.value.courtId = courts.value[0].id
    const b = await bookingsAPI.getAll()
    bookings.value = b.data
  } catch (err) {
    console.error(err)
  } finally {
    loading.value = false
  }
}

const checkAvailability = async () => {
  if (!form.value.startTime || !form.value.endTime) return alert('Chọn ngày giờ')
  try {
    const res = await bookingsAPI.checkAvailability(form.value.courtId, new Date(form.value.startTime).toISOString(), new Date(form.value.endTime).toISOString())
    available.value = res.data.isAvailable
  } catch (err) {
    console.error(err)
    available.value = null
  }
}

const createBooking = async () => {
  try {
    const res = await bookingsAPI.create({ courtId: form.value.courtId, startTime: new Date(form.value.startTime).toISOString(), endTime: new Date(form.value.endTime).toISOString() })
    alert('Đặt thành công. Chờ quản trị duyệt.')
    fetchData()
  } catch (err) {
    console.error(err)
    alert('Không thể đặt. Có thể cần đăng nhập.')
  }
}

const approve = async (id) => {
  try {
    await bookingsAPI.approve(id)
    alert('Đã duyệt')
    fetchData()
  } catch (err) {
    console.error(err)
    alert('Không thể duyệt')
  }
}

const reject = async (id) => {
  try {
    await bookingsAPI.reject(id)
    alert('Đã từ chối')
    fetchData()
  } catch (err) {
    console.error(err)
    alert('Không thể từ chối')
  }
}

onMounted(fetchData)

const pendingBookings = computed(() => bookings.value.filter(b => b.status === 'Pending'))
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
