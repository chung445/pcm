import { defineStore } from 'pinia'
import { authAPI } from '@/services/api'

export const useAuthStore = defineStore('auth', {
  state: () => ({
    user: JSON.parse(localStorage.getItem('user')) || null,
    token: localStorage.getItem('token') || null,
    isAuthenticated: !!localStorage.getItem('token')
  }),

  getters: {
    isAdmin: (state) => state.user?.roles?.includes('Admin') || false,
    isTreasurer: (state) => state.user?.roles?.includes('Treasurer') || false,
    isReferee: (state) => state.user?.roles?.includes('Referee') || false,
    isMember: (state) => state.user?.roles?.includes('Member') || false,
    currentMemberId: (state) => state.user?.memberId || null,
    userName: (state) => state.user?.fullName || ''
  },

  actions: {
    async login(credentials) {
      try {
        const response = await authAPI.login(credentials)
        const { token, ...userData } = response.data

        this.token = token
        this.user = userData
        this.isAuthenticated = true

        localStorage.setItem('token', token)
        localStorage.setItem('user', JSON.stringify(userData))

        return true
      } catch (error) {
        console.error('Login failed:', error)
        throw error
      }
    },

    async register(userData) {
      try {
        const response = await authAPI.register(userData)
        const { token, ...user } = response.data

        this.token = token
        this.user = user
        this.isAuthenticated = true

        localStorage.setItem('token', token)
        localStorage.setItem('user', JSON.stringify(user))

        return true
      } catch (error) {
        console.error('Registration failed:', error)
        throw error
      }
    },

    async getCurrentUser() {
      try {
        const response = await authAPI.getCurrentUser()
        this.user = response.data
        localStorage.setItem('user', JSON.stringify(response.data))
      } catch (error) {
        console.error('Failed to get current user:', error)
        this.logout()
      }
    },

    logout() {
      this.user = null
      this.token = null
      this.isAuthenticated = false

      localStorage.removeItem('token')
      localStorage.removeItem('user')
    }
  }
})
