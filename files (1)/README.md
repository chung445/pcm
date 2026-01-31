# Hệ Thống Quản Lý CLB Pickleball "Vợt Thủ Phố Núi" (PCM)

## 🎾 Giới thiệu

Đây là hệ thống quản lý toàn diện cho CLB Pickleball "Vợt Thủ Phố Núi", được xây dựng theo tinh thần **"Vui - Khỏe - Có Thưởng"**.

### Công nghệ sử dụng

**Backend:**
- ASP.NET Core 8.0 Web API
- Entity Framework Core 8.0
- SQL Server / SQL Server LocalDB
- Identity Framework (Authentication & Authorization)
- JWT (JSON Web Token)
- Swagger/OpenAPI

**Frontend:**
- Vue.js 3 (Composition API)
- Vue Router 4
- Pinia (State Management)
- Axios
- Bootstrap 5
- Vite

## 📁 Cấu trúc Project

```
PCM-Project/
├── Backend/                 # ASP.NET Core Web API
│   ├── Controllers/        # API Controllers
│   ├── Models/            # Entity Models
│   ├── Data/              # DbContext & DbInitializer
│   ├── Services/          # Business Logic Services
│   ├── DTOs/              # Data Transfer Objects
│   ├── Program.cs         # Entry point
│   ├── appsettings.json   # Configuration
│   └── README.md          # Backend documentation
│
└── Frontend/               # Vue.js 3 Application
    ├── src/
    │   ├── views/         # Page components
    │   ├── components/    # Reusable components
    │   ├── router/        # Vue Router
    │   ├── stores/        # Pinia stores
    │   ├── services/      # API services
    │   └── assets/        # CSS, images
    ├── index.html
    ├── vite.config.js
    └── README.md          # Frontend documentation
```

## 🚀 Hướng dẫn Cài đặt & Chạy

### Yêu cầu hệ thống

- **.NET 8.0 SDK** - [Download](https://dotnet.microsoft.com/download/dotnet/8.0)
- **Node.js 18+** - [Download](https://nodejs.org/)
- **SQL Server** hoặc **SQL Server LocalDB**
- **Visual Studio 2022** (khuyến nghị) hoặc **VS Code**

### Bước 1: Clone Repository

```bash
git clone <your-repo-url>
cd PCM-Project
```

### Bước 2: Setup Backend

```bash
cd Backend

# Restore dependencies
dotnet restore

# Update database connection string trong appsettings.json nếu cần
# Mặc định sử dụng LocalDB:
# "Server=(localdb)\\mssqllocaldb;Database=PCM_PickleballDB;Trusted_Connection=true"

# Tạo database và seed data
dotnet ef database update

# Chạy API
dotnet run
```

Backend API sẽ chạy tại: `http://localhost:5000` hoặc `https://localhost:7000`

Swagger UI: `http://localhost:5000/swagger`

### Bước 3: Setup Frontend

Mở terminal mới:

```bash
cd Frontend

# Cài đặt dependencies
npm install

# Chạy development server
npm run dev
```

Frontend sẽ chạy tại: `http://localhost:5173`

### Bước 4: Đăng nhập

Sử dụng tài khoản demo:

**Admin:**
- Email: `admin@pcm.com`
- Password: `Admin@123`

**Member:**
- Email: `an@pcm.com` (hoặc binh, cuong, dung, em, phuong, quan, hoa)
- Password: `Member@123`

## 📊 Tính năng chính

### 1. Quản trị Nội bộ (Operations)
- ✅ Quản lý Hội viên với Rank DUPR động
- ✅ Quản lý Tài chính Thu/Chi minh bạch
- ✅ Cảnh báo đỏ khi Quỹ âm
- ✅ Tin tức & Thông báo (có tính năng ghim)

### 2. Hoạt động Thường nhật
- ✅ Đặt sân (Booking) với kiểm tra trùng lịch
- ✅ Trận đấu Giao hữu (Daily Matches)
- ✅ Tự động cập nhật Rank DUPR sau mỗi trận

### 3. Sàn đấu & Sự kiện
- ✅ Kèo Thách đấu (Duel)
- ✅ Giải đấu Mini (Mini-game)
  - Team Battle: Chia 2 phe, đánh chạm mốc thắng
  - Round Robin: Đánh vòng tròn tích điểm cá nhân
- ✅ Entry Fee & Prize Pool
- ✅ Tự động chia team theo Rank

### 4. Phân quyền
- **Admin**: Toàn quyền
- **Treasurer**: Quản lý tài chính
- **Referee**: Ghi nhận trận đấu
- **Member**: Đặt sân, tham gia challenges, sửa thông tin cá nhân

## 🎯 Nghiệp vụ đặc biệt

### Quản lý Rank DUPR
- Mỗi hội viên có điểm Rank khởi đầu 3.0
- Thắng trận ranked: +0.1 điểm
- Thua trận ranked: -0.1 điểm (không âm)
- Top ranking hiển thị trên Dashboard

### Kiểm tra Trùng lịch Đặt sân
- Hệ thống tự động chặn đặt sân trùng giờ
- Ví dụ: Sân 1 đã đặt 8:00-9:00 → Không thể đặt 8:30-9:30

### Team Battle Challenge
- Chia đều 2 phe A-B theo rank (zigzag)
- Tính điểm theo số trận thắng của từng phe
- Tự động kết thúc khi đạt target wins
- Phân phối prize pool cho phe thắng

### Cảnh báo Quỹ Âm
- Hiển thị cảnh báo đỏ trên Dashboard
- Số dư quỹ = Tổng Thu - Tổng Chi
- Tự động tính toán realtime

## 📱 Screenshots

*(Thêm screenshots của ứng dụng nếu có)*

## 🗄️ Database Schema

Tất cả bảng nghiệp vụ có prefix `999_` (3 số cuối MSSV):

- `999_Members` - Hội viên
- `999_News` - Tin tức
- `999_TransactionCategories` - Danh mục thu/chi
- `999_Transactions` - Giao dịch tài chính
- `999_Courts` - Sân
- `999_Bookings` - Đặt sân
- `999_Challenges` - Giải đấu/Kèo
- `999_Matches` - Trận đấu
- `999_Participants` - Người tham gia challenges
- `999_MatchScores` - Điểm số chi tiết (optional)
- `999_Notifications` - Thông báo (optional)

## 🧪 Testing

### Test Backend API với Swagger

1. Chạy Backend
2. Mở `http://localhost:5000/swagger`
3. Đăng nhập qua endpoint `/api/auth/login`
4. Copy token từ response
5. Click "Authorize" button, nhập: `Bearer {token}`
6. Test các endpoints

### Test Frontend

1. Đăng nhập với tài khoản demo
2. Thử các tính năng:
   - Xem Dashboard
   - Xem danh sách Members
   - Đặt sân
   - Tham gia Challenge
   - Ghi nhận Match (Admin/Referee)

## 🐛 Troubleshooting

### Backend không chạy được
```bash
# Xóa database cũ và tạo lại
dotnet ef database drop
dotnet ef database update
```

### Frontend không kết nối được API
- Kiểm tra Backend đang chạy
- Kiểm tra CORS trong `Program.cs`
- Kiểm tra API URL trong `Frontend/src/services/api.js`

### Lỗi Migration
```bash
cd Backend
dotnet ef migrations remove
dotnet ef migrations add InitialCreate
dotnet ef database update
```

## 📚 Tài liệu tham khảo

- **Backend README**: [Backend/README.md](Backend/README.md)
- **Frontend README**: [Frontend/README.md](Frontend/README.md)
- **API Documentation**: Swagger UI tại `/swagger`

## 🎓 Học tập & Phát triển

Project này được tạo cho mục đích học tập môn **Fullstack Development**.

### Các điểm học được:

1. **Backend**: RESTful API, EF Core, Identity, JWT, Repository Pattern
2. **Frontend**: Vue 3 Composition API, State Management, Routing, API Integration
3. **Full-stack**: Authentication flow, CRUD operations, Real-time updates
4. **DevOps**: Git workflow, Project structure, Documentation

### Mở rộng có thể phát triển:

- [ ] Real-time notifications với SignalR
- [ ] Calendar view cho booking
- [ ] Export báo cáo Excel/PDF
- [ ] Upload avatar cho members
- [ ] Charts & Analytics
- [ ] Mobile app với React Native / Flutter
- [ ] CI/CD với GitHub Actions
- [ ] Deploy lên Azure / AWS

## 👥 Đóng góp

Project này mở để học tập. Bạn có thể fork và phát triển thêm các tính năng mới!

## 📄 License

MIT License - Free to use for educational purposes

---

## 📞 Liên hệ

Nếu có thắc mắc, vui lòng liên hệ qua:
- Email: [your-email@example.com]
- GitHub: [your-github-username]

---

**Chúc bạn học tốt và code vui vẻ! 🎾🚀**
