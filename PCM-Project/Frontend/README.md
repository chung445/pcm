# PCM Pickleball Club Management System - Frontend

## Hệ thống Quản lý CLB Pickleball "Vợt Thủ Phố Núi" - Giao diện người dùng

### Công nghệ sử dụng
- Vue.js 3 (Composition API)
- Vue Router 4
- Pinia (State Management)
- Axios (HTTP Client)
- Bootstrap 5
- Bootstrap Icons
- Vue Toastification (Notifications)
- Vite (Build Tool)

### Cài đặt và Chạy Project

#### Bước 1: Cài đặt Dependencies
```bash
npm install
```

#### Bước 2: Cấu hình API Endpoint
Mở file `src/services/api.js` và kiểm tra API base URL:
```javascript
const API_BASE_URL = 'http://localhost:5000/api'
```

**Lưu ý:** Đảm bảo Backend API đang chạy ở địa chỉ này. Nếu Backend chạy ở port khác, cập nhật URL tương ứng.

#### Bước 3: Chạy Development Server
```bash
npm run dev
```

Frontend sẽ chạy tại: `http://localhost:5173`

#### Bước 4: Build cho Production
```bash
npm run build
```

Các file build sẽ được tạo trong thư mục `dist/`

### Tài khoản Demo

Sử dụng các tài khoản đã được seed trong Backend:

#### Admin:
- **Email:** admin@pcm.com
- **Password:** Admin@123

#### Members:
- **Email:** an@pcm.com, binh@pcm.com, cuong@pcm.com, dung@pcm.com, em@pcm.com, phuong@pcm.com, quan@pcm.com, hoa@pcm.com
- **Password:** Member@123

### Cấu trúc Project

```
Frontend/
├── public/              # Static files
├── src/
│   ├── assets/         # CSS, images
│   ├── components/     # Reusable components
│   ├── views/          # Page components
│   ├── router/         # Vue Router config
│   ├── stores/         # Pinia stores
│   ├── services/       # API services
│   ├── utils/          # Utility functions
│   ├── App.vue         # Root component
│   └── main.js         # Entry point
├── index.html
├── vite.config.js
└── package.json
```

### Tính năng chính

#### 1. Authentication (Xác thực)
- Đăng nhập / Đăng ký
- JWT Token authentication
- Auto-refresh token
- Protected routes (chỉ cho phép user đã đăng nhập)
- Role-based access control (Admin, Treasurer, Referee, Member)

#### 2. Dashboard
- Thống kê tổng quan: Số hội viên, Challenges, Trận đấu, Quỹ CLB
- Cảnh báo quỹ âm (màu đỏ khi số dư < 0)
- Hiển thị tin tức ghim
- Top 5 Ranking (xếp hạng theo điểm DUPR)

#### 3. Quản lý Hội viên
- Xem danh sách tất cả hội viên
- Xem chi tiết từng hội viên
- Sửa thông tin cá nhân (chỉ được sửa của chính mình)
- Hiển thị Rank, tỷ lệ thắng, số trận

#### 4. Tin tức & Thông báo
- Xem danh sách tin tức
- Tin ghim hiển thị ưu tiên
- CRUD tin tức (chỉ Admin)

#### 5. Quản lý Sân
- Xem danh sách sân
- CRUD sân (chỉ Admin)

#### 6. Đặt sân (Booking)
- Xem lịch đặt sân
- Đặt sân mới
- Kiểm tra sân trống
- Hủy đặt sân (chỉ được hủy của mình)
- Validation: Không cho phép đặt trùng giờ

#### 7. Sàn đấu (Challenges)
- Xem danh sách Challenges
- Tạo Challenge mới (Duel / MiniGame)
- Tham gia Challenge
- Chia team tự động (Admin/Referee)
- Hiển thị Entry Fee, Prize Pool, Số người tham gia

#### 8. Trận đấu (Matches)
- Xem danh sách trận đấu
- Ghi nhận trận đấu mới (Admin/Referee)
- Chọn thể thức: Đơn (Singles) / Đôi (Doubles)
- Chọn người chơi từ danh sách members
- Ghi nhận kết quả, cập nhật Rank tự động

#### 9. Tài chính (Transactions)
- Xem danh sách giao dịch (Admin/Treasurer)
- Tạo giao dịch Thu/Chi
- Báo cáo tổng hợp: Tổng thu, Tổng chi, Số dư
- Quản lý danh mục Thu/Chi

### State Management với Pinia

#### Auth Store (`stores/auth.js`)
```javascript
- state: user, token, isAuthenticated
- getters: isAdmin, isTreasurer, isReferee, isMember
- actions: login, register, logout, getCurrentUser
```

### API Services

Tất cả các API calls được tổ chức trong `services/api.js`:

- `authAPI`: login, register, getCurrentUser
- `membersAPI`: getAll, getById, update, getTopRanking
- `newsAPI`: getAll, getById, create, update, delete
- `courtsAPI`: getAll, create, update, delete
- `bookingsAPI`: getAll, create, delete, checkAvailability
- `challengesAPI`: getAll, getById, create, join, autoDivideTeams
- `matchesAPI`: getAll, getById, create
- `transactionsAPI`: getAll, create, getSummary
- `categoriesAPI`: getAll, create, update, delete

### Axios Interceptors

#### Request Interceptor
- Tự động thêm JWT token vào header `Authorization: Bearer {token}` cho mọi request

#### Response Interceptor
- Tự động xử lý lỗi 401 (Unauthorized)
- Redirect về trang login khi token hết hạn
- Clear localStorage

### Vue Router Guards

```javascript
router.beforeEach((to, from, next) => {
  // Kiểm tra authentication
  // Redirect về login nếu chưa đăng nhập
  // Redirect về dashboard nếu đã đăng nhập nhưng vào trang login
})
```

### Toast Notifications

Sử dụng `vue-toastification` để hiển thị thông báo:

```javascript
import { useToast } from 'vue-toastification'

const toast = useToast()

toast.success('Thành công!')
toast.error('Có lỗi xảy ra!')
toast.warning('Cảnh báo!')
toast.info('Thông tin')
```

### Styling

- **Bootstrap 5**: Framework CSS chính
- **Bootstrap Icons**: Icons
- **Custom CSS**: `assets/main.css` cho styling riêng
- **Responsive**: Tất cả components đều responsive, mobile-friendly

### Environment Variables

Tạo file `.env.local` để cấu hình:

```
VITE_API_BASE_URL=http://localhost:5000/api
```

### Troubleshooting

#### Lỗi CORS
- Đảm bảo Backend đã cấu hình CORS cho `http://localhost:5173`
- Kiểm tra file `Program.cs` trong Backend

#### Lỗi kết nối API
- Kiểm tra Backend đang chạy
- Kiểm tra API base URL trong `services/api.js`
- Kiểm tra console browser để xem error details

#### Token hết hạn
- Token có thời hạn 24 giờ (1440 phút)
- Sau khi hết hạn, hệ thống tự động redirect về login
- Đăng nhập lại để lấy token mới

### Development Tips

1. **Hot Module Replacement (HMR)**: Vite hỗ trợ HMR, thay đổi code sẽ tự động reload
2. **Vue DevTools**: Cài extension Vue DevTools để debug
3. **Network Tab**: Dùng Network tab trong browser để xem API requests/responses
4. **Console Logging**: Tất cả errors đều được log ra console

### Build & Deployment

#### Build
```bash
npm run build
```

#### Preview build
```bash
npm run preview
```

#### Deploy
- Upload thư mục `dist/` lên server
- Hoặc deploy lên Vercel, Netlify, GitHub Pages, v.v.
- **Lưu ý**: Cần cấu hình API base URL cho production

### Mở rộng

Các tính năng có thể phát triển thêm:

1. **Calendar View cho Booking**: Dùng FullCalendar hoặc Vue Calendar
2. **Real-time Updates**: Dùng SignalR
3. **Export Reports**: Excel, PDF
4. **Advanced Filtering & Search**: Tìm kiếm nâng cao
5. **Image Upload**: Avatar cho members
6. **Charts & Graphs**: Biểu đồ thống kê
7. **Notifications Center**: Trung tâm thông báo
8. **Chat/Messaging**: Tin nhắn giữa các thành viên

### License
Project này được tạo cho mục đích học tập - Môn Fullstack Development

---

## Hướng dẫn Quick Start

1. Clone repo
2. Vào thư mục Frontend: `cd Frontend`
3. Cài dependencies: `npm install`
4. Chạy dev server: `npm run dev`
5. Mở browser: `http://localhost:5173`
6. Login với: admin@pcm.com / Admin@123

**Chúc bạn code vui vẻ! 🎾**
