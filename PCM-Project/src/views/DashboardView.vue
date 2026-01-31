<template>
  <div class="dashboard">
    <h1 class="mb-4">
      <i class="bi bi-speedometer2"></i> Dashboard
    </h1>

    <!-- Statistics Cards -->
    <div class="row mb-4">
      <div class="col-md-3">
        <div class="card stat-card text-white bg-primary">
          <div class="card-body">
            <h6 class="card-title"><i class="bi bi-people"></i> Tổng hội viên</h6>
            <h2>{{ stats.totalMembers }}</h2>
          </div>
        </div>
      </div>
      <div class="col-md-3">
        <div class="card stat-card text-white bg-success">
          <div class="card-body">
            <h6 class="card-title"><i class="bi bi-trophy"></i> Challenges</h6>
            <h2>{{ stats.activeChallenges }}</h2>
            <small>Đang mở</small>
          </div>
        </div>
      </div>
      <div class="col-md-3">
        <div class="card stat-card text-white bg-info">
          <div class="card-body">
            <h6 class="card-title"><i class="bi bi-clipboard-data"></i> Trận đấu</h6>
            <h2>{{ stats.totalMatches }}</h2>
            <small>Trong tháng</small>
          </div>
        </div>
      </div>
      <div class="col-md-3">
        <div class="card stat-card text-white" :class="treasury.isNegative ? 'bg-danger' : 'bg-warning'">
          <div class="card-body">
            <h6 class="card-title"><i class="bi bi-wallet2"></i> Quỹ CLB</h6>
            <h2>{{ formatCurrency(treasury.balance) }}</h2>
            <small v-if="treasury.isNegative" class="fw-bold">⚠️ CẢNHể BÁO QUỸ ÂM!</small>
          </div>
        </div>
      </div>
    </div>

    <div class="row">
      <!-- Pinned News -->
      <div class="col-md-8">
        <div class="card">
          <div class="card-header">
            <i class="bi bi-newspaper"></i> Tin tức ghim
          </div>
          <div class="card-body">
            <div v-if="loading.news" class="text-center py-3">
              <div class="spinner-border"></div>
            </div>
            <div v-else-if="pinnedNews.length === 0" class="text-center text-muted py-3">
              Chưa có tin tức ghim
            </div>
            <div v-else>
              <div v-for="news in pinnedNews" :key="news.id" class="mb-3 pinned-news p-3 bg-light rounded">
                <h5 class="mb-2">
                  <i class="bi bi-pin-fill text-warning"></i>
                  {{ news.title }}
                </h5>
                <p class="mb-1">{{ news.content }}</p>
                <small class="text-muted">
                  <i class="bi bi-clock"></i> {{ formatDate(news.createdDate) }}
                </small>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Top Ranking -->
      <div class="col-md-4">
        <div class="card">
          <div class="card-header">
            <i class="bi bi-trophy-fill"></i> Top 5 Ranking
          </div>
          <div class="card-body">
            <div v-if="loading.ranking" class="text-center py-3">
              <div class="spinner-border"></div>
            </div>
            <ul v-else class="top-ranking-list">
              <li v-for="(member, index) in topRanking" :key="member.id" class="top-ranking-item">
                <div class="d-flex align-items-center">
                  <div :class="['rank-badge me-3', getRankClass(index + 1)]">
                    {{ index + 1 }}
                  </div>
                  <div>
                    <div class="fw-bold">{{ member.fullName }}</div>
                    <small class="text-muted">
                      {{ member.winMatches }}/{{ member.totalMatches }} thắng
                    </small>
                  </div>
                </div>
                <div>
                  <span class="badge bg-primary badge-rank">{{ member.rankLevel.toFixed(1) }}</span>
                </div>
              </li>
            </ul>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { newsAPI, membersAPI, challengesAPI, matchesAPI, transactionsAPI } from '@/services/api'

const stats = ref({
  totalMembers: 0,
  activeChallenges: 0,
  totalMatches: 0
})

const treasury = ref({
  balance: 0,
  isNegative: false
})

const pinnedNews = ref([])
const topRanking = ref([])

const loading = ref({
  news: false,
  ranking: false
})

const formatCurrency = (value) => {
  return new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(value)
}

const formatDate = (date) => {
  return new Date(date).toLocaleDateString('vi-VN')
}

const getRankClass = (rank) => {
  if (rank === 1) return 'rank-1'
  if (rank === 2) return 'rank-2'
  if (rank === 3) return 'rank-3'
  return 'rank-other'
}

const loadData = async () => {
  try {
    // Load members
    const membersRes = await membersAPI.getAll()
    stats.value.totalMembers = membersRes.data.length

    // Load challenges
    const challengesRes = await challengesAPI.getAll()
    stats.value.activeChallenges = challengesRes.data.filter(c => c.status === 'Open' || c.status === 'Ongoing').length

    // Load matches
    const matchesRes = await matchesAPI.getAll()
    stats.value.totalMatches = matchesRes.data.length

    // Load treasury
    const treasuryRes = await transactionsAPI.getSummary()
    treasury.value = treasuryRes.data

    // Load pinned news
    loading.value.news = true
    const newsRes = await newsAPI.getAll(true)
    pinnedNews.value = newsRes.data

    // Load top ranking
    loading.value.ranking = true
    const rankingRes = await membersAPI.getTopRanking(5)
    topRanking.value = rankingRes.data
  } catch (error) {
    console.error('Error loading dashboard data:', error)
  } finally {
    loading.value.news = false
    loading.value.ranking = false
  }
}

onMounted(() => {
  loadData()
})
</script>

<style scoped>
.dashboard h1 {
  font-family: 'Montserrat', 'Roboto', Arial, sans-serif;
  font-weight: 700;
  letter-spacing: 1px;
  color: #059669;
}
.stat-card {
  min-height: 120px;
  display: flex;
  flex-direction: column;
  justify-content: center;
  align-items: flex-start;
  font-size: 1.2rem;
}
.card-header {
  background: #059669;
  color: #fff;
  font-weight: 600;
  border-radius: 1rem 1rem 0 0;
  font-size: 1.1rem;
}
.pinned-news {
  border-left: 4px solid #fbbf24;
  background: #f9fafb;
  box-shadow: 0 2px 8px rgba(0,0,0,0.04);
}
.badge {
  font-size: 1rem;
  border-radius: 0.5rem;
  padding: 0.5em 1em;
}
.top-ranking .card-header {
  background: #059669;
  color: #fff;
}
.top-ranking .badge {
  font-size: 1.1rem;
  font-weight: 700;
}
@media (max-width: 768px) {
  .dashboard h1 {
    font-size: 1.5rem;
  }
  .stat-card {
    font-size: 1rem;
    min-height: 80px;
  }
}
</style>
