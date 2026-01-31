# PCM Pickleball Club Management System

## 🎾 Giới thiệu
Hệ thống quản lý toàn diện cho CLB Pickleball "Vợt Thủ Phố Núi" – quản lý hội viên, sân, trận đấu, tài chính, tin tức, đặt sân, sàn đấu... với UI hiện đại, responsive, sử dụng .NET 8 và Vue 3 + Bootstrap 5.

## 🛠️ Công nghệ sử dụng
**Backend:**
- ASP.NET Core 8.0 Web API
- Entity Framework Core 8.0
- SQL Server / LocalDB
- Identity Framework, JWT
- Swagger/OpenAPI

**Frontend:**
- Vue.js 3 (Composition API)
- Vite
- Vue Router 4
- Pinia
- Axios
- Bootstrap 5
- Vue Toastification

## 📁 Cấu trúc Project
```
PCM-Project/
├── Backend/   # ASP.NET Core Web API
│   ├── Controllers/
│   ├── Models/
│   ├── Data/
│   ├── Services/
│   ├── DTOs/
│   ├── Program.cs
│   ├── appsettings.json
│   └── README.md
└── Frontend/  # Vue.js 3 + Bootstrap 5
    ├── src/
    │   ├── views/
    │   ├── components/
    │   ├── router/
    │   ├── stores/
    │   ├── services/
    │   └── assets/
    ├── index.html
    ├── vite.config.js
    └── README.md
```

## 🚀 Hướng dẫn Cài đặt & Chạy
### Yêu cầu hệ thống
- .NET 8.0 SDK
- Node.js 18+
- SQL Server hoặc LocalDB
- Visual Studio 2022 hoặc VS Code

### 1. Clone Repository
```bash
git clone <your-repo-url>
cd PCM-Project
```

### 2. Setup Backend
```bash
cd Backend
dotnet restore
dotnet ef database update
dotnet run
```
API chạy tại: http://localhost:5000
Swagger: http://localhost:5000/swagger

### 3. Setup Frontend
```bash
cd Frontend
npm install
npm run dev
```
Truy cập: http://localhost:5173

### 4. Đăng nhập demo
**Admin:** admin@pcm.com / Admin@123
**Member:** an@pcm.com, binh@pcm.com... / Member@123

## 📊 Tính năng chính
- Đăng nhập/Đăng ký, phân quyền (Admin, Treasurer, Referee, Member)
- Dashboard tổng quan, cảnh báo quỹ âm, top ranking
- Quản lý hội viên, xem/sửa thông tin, thống kê thành tích
- Quản lý sân, đặt sân, kiểm tra trùng giờ, hủy đặt sân
- Quản lý tin tức, thông báo, CRUD tin tức
- Sàn đấu (Challenge), tạo/join/chia team tự động
- Quản lý trận đấu, ghi nhận kết quả, cập nhật rank
- Quản lý tài chính, giao dịch, báo cáo thu/chi, danh mục

## 🎯 Nghiệp vụ đặc biệt
- Quản lý Rank DUPR động, tự động cập nhật sau mỗi trận
- Kiểm tra trùng lịch đặt sân
- Team Battle: Chia team tự động, tính điểm, chia thưởng
- Cảnh báo quỹ âm realtime

## 🧪 Testing
**Backend:**
1. Chạy Backend
2. Mở Swagger: http://localhost:5000/swagger
3. Đăng nhập, lấy token, test các endpoints

**Frontend:**
1. Đăng nhập với tài khoản demo
2. Thử các tính năng: Dashboard, Members, Booking, Challenge, Match

## 🐛 Troubleshooting
- Backend không chạy: Xóa DB cũ, chạy lại migration
- Frontend không kết nối API: Kiểm tra backend, CORS, API URL
- Lỗi migration: Xóa, tạo lại migration

## 📚 Tài liệu tham khảo
- Backend/README.md
- Frontend/README.md
- Swagger UI tại /swagger

## 🎓 Học tập & Phát triển
Project phục vụ học tập môn Fullstack Development.

**Học được:**
- Backend: RESTful API, EF Core, Identity, JWT
- Frontend: Vue 3, State Management, Routing, API Integration
- Full-stack: Auth flow, CRUD, Real-time
- DevOps: Git, Project structure, Documentation

**Mở rộng:**
- Real-time notifications (SignalR)
- Calendar view cho booking
- Export báo cáo Excel/PDF
- Upload avatar cho members
- Charts & Analytics
- Mobile app
- CI/CD, deploy cloud

## 👥 Đóng góp
Bạn có thể fork và phát triển thêm!

## 📄 License
MIT License - Free for educational use

---
**Chúc bạn học tốt và code vui vẻ! 🎾🚀**
