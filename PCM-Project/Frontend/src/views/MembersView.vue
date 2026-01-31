<!-- MembersView.vue -->
<template>
  <div>
    <h1 class="mb-4"><i class="bi bi-people"></i> Quản lý Hội viên</h1>
    <!-- Admin: Create/Edit Member -->
    <div v-if="isAdmin" class="card mb-3 p-3">
      <h5>{{ editing ? 'Chỉnh sửa hội viên' : 'Thêm hội viên mới' }}</h5>
      <div class="mb-2">
        <input v-model="form.fullName" class="form-control" placeholder="Họ tên" />
      </div>
      <div class="mb-2">
        <input v-model="form.email" class="form-control" placeholder="Email" />
      </div>
      <div class="mb-2">
        <input v-model="form.rankLevel" type="number" step="0.1" class="form-control" placeholder="Rank" />
      </div>
      <div class="mb-2">
        <input v-model="form.phoneNumber" class="form-control" placeholder="Số điện thoại" />
      </div>
      <div class="mb-2">
        <input v-model="form.dateOfBirth" type="date" class="form-control" placeholder="Ngày sinh" />
      </div>
      <div>
        <button class="btn btn-primary me-2" @click="editing ? updateMember() : createMember()">{{ editing ? 'Cập nhật' : 'Thêm' }}</button>
        <button v-if="editing" class="btn btn-secondary me-2" @click="cancelEdit()">Hủy</button>
      </div>
    </div>
    <div class="card">
      <div class="card-body">
        <div v-if="loading" class="text-center"><div class="spinner-border"></div></div>
        <table v-else class="table table-hover">
          <thead>
            <tr>
              <th>Họ tên</th>
              <th>Email</th>
              <th>Rank</th>
              <th>Trận đấu</th>
              <th>Tỷ lệ thắng</th>
              <th>Trạng thái</th>
              <th v-if="isAdmin">Hành động</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="member in members" :key="member.id">
              <td>{{ member.fullName }}</td>
              <td>{{ member.email }}</td>
              <td><span class="badge bg-primary">{{ member.rankLevel.toFixed(1) }}</span></td>
              <td>{{ member.totalMatches }}</td>
              <td>{{ member.winRate?.toFixed(1) || 0 }}%</td>
              <td><span class="badge bg-success">{{ member.status }}</span></td>
              <td v-if="isAdmin">
                <button class="btn btn-sm btn-outline-primary me-2" @click="startEdit(member)">Sửa</button>
                <button class="btn btn-sm btn-danger" @click="deleteMember(member.id)">Xóa</button>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue'
import { membersAPI } from '@/services/api'
import { useAuthStore } from '@/stores/auth'

const auth = useAuthStore()
const isAdmin = computed(() => auth.isAdmin)

const members = ref([])
const loading = ref(false)
const editing = ref(false)
const form = ref({ id: null, fullName: '', email: '', rankLevel: 3.0, phoneNumber: '', dateOfBirth: '' })

const fetchMembers = async () => {
  loading.value = true
  try {
    const res = await membersAPI.getAll()
    members.value = res.data
  } catch (error) {
    console.error(error)
  } finally {
    loading.value = false
  }
}

const createMember = async () => {
  try {
    await membersAPI.create({
      fullName: form.value.fullName,
      email: form.value.email,
      rankLevel: form.value.rankLevel,
      phoneNumber: form.value.phoneNumber,
      dateOfBirth: form.value.dateOfBirth
    })
    form.value = { id: null, fullName: '', email: '', rankLevel: 3.0, phoneNumber: '', dateOfBirth: '' }
    fetchMembers()
    alert('Thêm hội viên thành công')
  } catch (err) {
    console.error(err)
    alert('Không thể thêm hội viên. Bạn có phải Admin?')
  }
}

const startEdit = (member) => {
  editing.value = true
  form.value.id = member.id
  form.value.fullName = member.fullName
  form.value.email = member.email
  form.value.rankLevel = member.rankLevel
  form.value.phoneNumber = member.phoneNumber
  form.value.dateOfBirth = member.dateOfBirth?.split('T')[0] || ''
  window.scrollTo({ top: 0, behavior: 'smooth' })
}

const cancelEdit = () => {
  editing.value = false
  form.value = { id: null, fullName: '', email: '', rankLevel: 3.0, phoneNumber: '', dateOfBirth: '' }
}

const updateMember = async () => {
  try {
    await membersAPI.update(form.value.id, {
      fullName: form.value.fullName,
      email: form.value.email,
      rankLevel: form.value.rankLevel,
      phoneNumber: form.value.phoneNumber,
      dateOfBirth: form.value.dateOfBirth
    })
    cancelEdit()
    fetchMembers()
    alert('Cập nhật thành công')
  } catch (err) {
    console.error(err)
    alert('Không thể cập nhật hội viên')
  }
}

const deleteMember = async (id) => {
  if (!confirm('Bạn có chắc chắn muốn xóa hội viên này?')) return
  try {
    await membersAPI.delete(id)
    fetchMembers()
    alert('Đã xóa hội viên')
  } catch (err) {
    console.error(err)
    alert('Không thể xóa hội viên')
  }
}

onMounted(fetchMembers)
</script>

<style scoped>
.card {
  border-radius: 1.25rem;
  box-shadow: 0 4px 24px rgba(0,0,0,0.08);
  border: none;
}
.table {
  border-radius: 1rem;
  background: #fff;
  box-shadow: 0 2px 12px rgba(0,0,0,0.06);
}
th, td {
  vertical-align: middle !important;
  font-size: 1rem;
}
.badge {
  font-size: 1rem;
  border-radius: 0.5rem;
  padding: 0.5em 1em;
}
.btn {
  border-radius: 0.75rem;
  font-size: 1rem;
}
@media (max-width: 768px) {
  .card, .table {
    border-radius: 0.75rem;
  }
}
</style>
