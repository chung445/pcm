import axios from 'axios'

let API_BASE_URL = import.meta.env.VITE_API_BASE_URL || 'http://localhost:5000/api'

// Xử lý URL: xóa khoảng trắng và dấu gạch chéo cuối để tránh lỗi //api
API_BASE_URL = API_BASE_URL.trim().replace(/\/+$/, '')

// Tự động thêm /api nếu thiếu
if (!API_BASE_URL.endsWith('/api')) {
  API_BASE_URL += '/api'
}

console.log('Current API Base URL:', API_BASE_URL)

const apiClient = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json'
  }
})

// Request interceptor to add token
apiClient.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem('token')
    if (token) {
      config.headers.Authorization = `Bearer ${token}`
    }
    return config
  },
  (error) => {
    return Promise.reject(error)
  }
)

// Response interceptor to handle errors
apiClient.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      localStorage.removeItem('token')
      localStorage.removeItem('user')
      window.location.href = '/login'
    }
    return Promise.reject(error)
  }
)

// Auth API
export const authAPI = {
  login: (credentials) => apiClient.post('/auth/login', credentials),
  register: (userData) => apiClient.post('/auth/register', userData),
  getCurrentUser: () => apiClient.get('/auth/me')
}

// Members API
export const membersAPI = {
  getAll: () => apiClient.get('/members'),
  getById: (id) => apiClient.get(`/members/${id}`),
  update: (id, data) => apiClient.put(`/members/${id}`, data),
  getTopRanking: (limit = 5) => apiClient.get(`/members/top-ranking?limit=${limit}`)
}

// News API
export const newsAPI = {
  getAll: (isPinned) => apiClient.get(`/news${isPinned !== undefined ? `?isPinned=${isPinned}` : ''}`),
  getById: (id) => apiClient.get(`/news/${id}`),
  create: (data) => apiClient.post('/news', data),
  update: (id, data) => apiClient.put(`/news/${id}`, data),
  delete: (id) => apiClient.delete(`/news/${id}`)
}

// Courts API
export const courtsAPI = {
  getAll: () => apiClient.get('/courts'),
  create: (data) => apiClient.post('/courts', data),
  update: (id, data) => apiClient.put(`/courts/${id}`, data),
  delete: (id) => apiClient.delete(`/courts/${id}`)
}

// Bookings API
export const bookingsAPI = {
  getAll: () => apiClient.get('/bookings'),
  create: (data) => apiClient.post('/bookings', data),
  delete: (id) => apiClient.delete(`/bookings/${id}`),
  checkAvailability: (courtId, startTime, endTime) => 
    apiClient.get(`/bookings/available-slots?courtId=${courtId}&startTime=${startTime}&endTime=${endTime}`),
  approve: (id) => apiClient.put(`/bookings/${id}/approve`),
  reject: (id) => apiClient.put(`/bookings/${id}/reject`)
}

// Challenges API
export const challengesAPI = {
  getAll: () => apiClient.get('/challenges'),
  getById: (id) => apiClient.get(`/challenges/${id}`),
  create: (data) => apiClient.post('/challenges', data),
  join: (id) => apiClient.post(`/challenges/${id}/join`),
  autoDivideTeams: (id) => apiClient.post(`/challenges/${id}/auto-divide-teams`)
}

// Matches API
export const matchesAPI = {
  getAll: () => apiClient.get('/matches'),
  getById: (id) => apiClient.get(`/matches/${id}`),
  create: (data) => apiClient.post('/matches', data)
}

// Transactions API
export const transactionsAPI = {
  getAll: () => apiClient.get('/transactions'),
  create: (data) => apiClient.post('/transactions', data),
  getSummary: () => apiClient.get('/transactions/summary'),
  approve: (id) => apiClient.put(`/transactions/${id}/approve`),
  reject: (id) => apiClient.put(`/transactions/${id}/reject`)
}

// Transaction Categories API
export const categoriesAPI = {
  getAll: () => apiClient.get('/transaction-categories'),
  create: (data) => apiClient.post('/transaction-categories', data),
  update: (id, data) => apiClient.put(`/transaction-categories/${id}`, data),
  delete: (id) => apiClient.delete(`/transaction-categories/${id}`)
}

export default apiClient
