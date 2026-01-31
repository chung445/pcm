# 🚀 HƯỚNG DẪN CHẠY NHANH - PCM Pickleball Club

## Các bước thực hiện (5-10 phút)

### 1️⃣ Giải nén và mở project

```bash
# Giải nén file
tar -xzf PCM-Project.tar.gz

# hoặc unzip nếu là file .zip
# unzip PCM-Project.zip
```

### 2️⃣ Chạy Backend (Terminal 1)

```bash
cd PCM-Project/Backend

# Restore packages
dotnet restore

# Tạo database (chỉ lần đầu)
dotnet ef database update

# Chạy API
dotnet run
```

✅ Backend chạy tại: `http://localhost:5000`  
✅ Swagger UI: `http://localhost:5000/swagger`

**Lưu ý:** Nếu gặp lỗi migration:
```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### 3️⃣ Chạy Frontend (Terminal 2 - Terminal mới)

```bash
cd PCM-Project/Frontend

# Cài đặt packages (chỉ lần đầu)
npm install

# Chạy dev server
npm run dev
```

✅ Frontend chạy tại: `http://localhost:5173`

### 4️⃣ Đăng nhập

Mở trình duyệt: `http://localhost:5173`

**Tài khoản Admin:**
- Email: `admin@pcm.com`
- Password: `Admin@123`

**Tài khoản Member:**
- Email: `an@pcm.com`
- Password: `Member@123`

---

## 🎯 Tính năng chính cần test

### Dashboard
- ✅ Xem thống kê: Hội viên, Challenges, Matches, Quỹ
- ✅ Xem tin tức ghim
- ✅ Xem Top 5 Ranking

### Hội viên
- ✅ Xem danh sách members
- ✅ Xem chi tiết member
- ✅ Sửa thông tin (chỉ của mình)

### Đặt sân
- ✅ Xem lịch đặt sân
- ✅ Đặt sân mới
- ✅ Test trùng lịch (thử đặt 2 booking cùng giờ)

### Sàn đấu
- ✅ Xem danh sách Challenges
- ✅ Tạo Challenge mới
- ✅ Tham gia Challenge

### Trận đấu (Admin/Referee only)
- ✅ Ghi nhận trận đấu mới
- ✅ Chọn Singles/Doubles
- ✅ Xem Rank tự động cập nhật

### Tài chính (Admin/Treasurer only)
- ✅ Xem giao dịch
- ✅ Tạo giao dịch mới
- ✅ Xem báo cáo tổng hợp

---

## ⚙️ Yêu cầu hệ thống

### Bắt buộc:
- ✅ .NET 8.0 SDK - https://dotnet.microsoft.com/download/dotnet/8.0
- ✅ Node.js 18+ - https://nodejs.org/
- ✅ SQL Server hoặc SQL Server LocalDB

### Khuyến nghị:
- Visual Studio 2022 hoặc VS Code
- SQL Server Management Studio (SSMS)
- Postman (để test API)

---

## 🐛 Xử lý lỗi thường gặp

### Backend: "Cannot open database"
```bash
# Kiểm tra SQL Server đang chạy
# Hoặc thay đổi connection string trong appsettings.json
```

### Frontend: "Network Error"
```bash
# Kiểm tra Backend đang chạy
# Kiểm tra URL trong src/services/api.js
```

### CORS Error
```bash
# Đảm bảo Backend có CORS config cho localhost:5173
# Đã config sẵn trong Program.cs
```

---

## 📁 Cấu trúc Project

```
PCM-Project/
├── Backend/          # ASP.NET Core API
│   ├── Controllers/  # API endpoints
│   ├── Models/       # Database models
│   ├── Services/     # Business logic
│   └── Data/         # DbContext
│
└── Frontend/         # Vue.js 3
    ├── src/
    │   ├── views/    # Pages
    │   ├── stores/   # Pinia state
    │   └── services/ # API calls
    └── package.json
```

---

## 🎓 Dữ liệu mẫu đã có sẵn

Sau khi chạy `dotnet ef database update`, database sẽ có:

- **9 Users**: 1 Admin + 8 Members
- **2 Courts**: Sân 1, Sân 2
- **5 Transaction Categories**: Tiền sân, Quỹ tháng, Nước, Phạt, Thưởng giải
- **4 Transactions**: Tổng quỹ hiện tại ~1,150,000 VNĐ
- **3 News**: 2 tin ghim, 1 tin thường
- **1 Challenge**: Mini-game Team Battle đang diễn ra
- **3 Matches**: Đã có kết quả và cập nhật rank

---

## 📞 Cần hỗ trợ?

Xem thêm:
- README.md (tổng quan)
- Backend/README.md (chi tiết Backend)
- Frontend/README.md (chi tiết Frontend)
- Swagger UI: http://localhost:5000/swagger

---

**Happy Coding! 🎾✨**
