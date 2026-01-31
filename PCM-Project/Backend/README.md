# PCM Pickleball Club Management System - Backend API

## Hệ thống Quản lý CLB Pickleball "Vợt Thủ Phố Núi"

### Công nghệ sử dụng
- ASP.NET Core 8.0 Web API
- Entity Framework Core 8.0
- SQL Server
- Identity Framework
- JWT Authentication
- Swagger/OpenAPI

### Cài đặt và Chạy Project

#### Bước 1: Cài đặt Dependencies
```bash
dotnet restore
```

#### Bước 2: Cấu hình Database
Mở file `appsettings.json` và cập nhật connection string nếu cần:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=PCM_PickleballDB;Trusted_Connection=true;MultipleActiveResultSets=true"
  }
}
```

**Lưu ý:** Nếu bạn sử dụng SQL Server thay vì LocalDB, thay đổi connection string như sau:
```
"Server=localhost;Database=PCM_PickleballDB;User Id=sa;Password=YourPassword;TrustServerCertificate=true"
```

#### Bước 3: Chạy Migration và Seed Database
```bash
# Tạo migration đầu tiên
dotnet ef migrations add InitialCreate

# Update database (sẽ tự động seed dữ liệu mẫu)
dotnet ef database update
```

#### Bước 4: Chạy API
```bash
dotnet run
```

hoặc sử dụng Visual Studio: Nhấn F5

API sẽ chạy tại: `https://localhost:7xxx` hoặc `http://localhost:5xxx`

#### Bước 5: Truy cập Swagger UI
Mở trình duyệt và truy cập:
```
https://localhost:7xxx/swagger
```

### Dữ liệu Seed Mẫu

Sau khi chạy migration, hệ thống sẽ tự động tạo dữ liệu mẫu:

#### Tài khoản Admin:
- **Email:** admin@pcm.com
- **Password:** Admin@123
- **Role:** Admin

#### Tài khoản Member mẫu (8 tài khoản):
- **Email:** an@pcm.com, binh@pcm.com, cuong@pcm.com, dung@pcm.com, em@pcm.com, phuong@pcm.com, quan@pcm.com, hoa@pcm.com
- **Password:** Member@123
- **Role:** Member

#### Dữ liệu khác:
- 2 Sân (Courts): Sân 1, Sân 2
- 5 Transaction Categories: Tiền sân (Thu), Quỹ tháng (Thu), Nước (Chi), Phạt (Chi), Thưởng giải (Chi)
- 4 Transactions mẫu (Số dư quỹ hiện tại: 1,150,000 VNĐ)
- 3 News (tin tức): 2 tin ghim, 1 tin thường
- 1 Challenge (Mini-game Team Battle) đang diễn ra với 10 participants
- 3 Matches đã thi đấu

### API Endpoints

#### Authentication
- `POST /api/auth/login` - Đăng nhập
- `POST /api/auth/register` - Đăng ký (tạo tài khoản Member mới)
- `GET /api/auth/me` - Lấy thông tin user hiện tại (cần token)

#### Members
- `GET /api/members` - Danh sách tất cả members
- `GET /api/members/{id}` - Chi tiết member
- `PUT /api/members/{id}` - Cập nhật thông tin member (chỉ được sửa của chính mình)
- `GET /api/members/top-ranking?limit=5` - Top ranking members

#### News
- `GET /api/news` - Danh sách tin tức
- `GET /api/news?isPinned=true` - Lấy tin ghim
- `GET /api/news/{id}` - Chi tiết tin
- `POST /api/news` - Tạo tin mới (Admin only)
- `PUT /api/news/{id}` - Cập nhật tin (Admin only)
- `DELETE /api/news/{id}` - Xóa tin (Admin only)

#### Courts
- `GET /api/courts` - Danh sách sân
- `POST /api/courts` - Tạo sân mới (Admin only)
- `PUT /api/courts/{id}` - Cập nhật sân (Admin only)
- `DELETE /api/courts/{id}` - Xóa sân (Admin only)

#### Bookings
- `GET /api/bookings` - Danh sách đặt sân
- `POST /api/bookings` - Đặt sân mới
- `DELETE /api/bookings/{id}` - Hủy đặt sân (chỉ được hủy của mình)
- `GET /api/bookings/available-slots?courtId=1&startTime=...&endTime=...` - Kiểm tra sân trống

#### Challenges
- `GET /api/challenges` - Danh sách challenges
- `GET /api/challenges/{id}` - Chi tiết challenge
- `POST /api/challenges` - Tạo challenge mới
- `POST /api/challenges/{id}/join` - Tham gia challenge
- `POST /api/challenges/{id}/auto-divide-teams` - Chia team tự động (Admin/Referee only)

#### Matches
- `GET /api/matches` - Danh sách trận đấu
- `GET /api/matches/{id}` - Chi tiết trận đấu
- `POST /api/matches` - Tạo/ghi nhận trận đấu mới (Admin/Referee only)

#### Transactions
- `GET /api/transactions` - Danh sách giao dịch
- `POST /api/transactions` - Tạo giao dịch mới (Admin/Treasurer only)
- `GET /api/transactions/summary` - Tóm tắt tài chính (tổng thu, chi, số dư)

#### Transaction Categories
- `GET /api/transaction-categories` - Danh sách categories
- `POST /api/transaction-categories` - Tạo category mới (Admin only)
- `PUT /api/transaction-categories/{id}` - Cập nhật category (Admin only)
- `DELETE /api/transaction-categories/{id}` - Xóa category (Admin only)

### JWT Authentication

Để sử dụng các endpoint cần authentication:

1. Đăng nhập qua `POST /api/auth/login`
2. Lấy token từ response
3. Thêm token vào header của các request tiếp theo:
   ```
   Authorization: Bearer {your-token-here}
   ```

### Roles và Permissions

- **Admin**: Toàn quyền (CRUD tất cả)
- **Treasurer**: Quản lý tài chính (CRUD Transactions)
- **Referee**: Ghi nhận trận đấu (Create Matches)
- **Member**: Xem thông tin, đặt sân, tham gia challenges, sửa thông tin cá nhân

### Cấu trúc Database

Tất cả bảng nghiệp vụ bắt đầu bằng prefix `999_` (3 số cuối MSSV):

- `999_Members` - Thông tin hội viên
- `999_News` - Tin tức, thông báo
- `999_TransactionCategories` - Danh mục thu/chi
- `999_Transactions` - Giao dịch tài chính
- `999_Courts` - Sân pickleball
- `999_Bookings` - Đặt sân
- `999_Challenges` - Giải đấu, kèo thách đấu
- `999_Matches` - Trận đấu
- `999_Participants` - Người tham gia challenges
- `999_MatchScores` - Điểm số từng set (optional)
- `999_Notifications` - Thông báo (optional)

### Các Tính năng Đặc biệt

1. **Kiểm tra trùng lịch đặt sân**: Hệ thống tự động kiểm tra và không cho phép đặt trùng giờ trên cùng một sân

2. **Tự động cập nhật Rank**: Khi ghi nhận trận đấu ranked, hệ thống tự động cập nhật điểm DUPR:
   - Thắng: +0.1 điểm
   - Thua: -0.1 điểm (không âm)

3. **Cập nhật điểm Challenge**: Khi ghi nhận trận đấu thuộc Challenge Team Battle, hệ thống tự động:
   - Cập nhật điểm cho TeamA hoặc TeamB
   - Kết thúc Challenge khi đạt target wins

4. **Cảnh báo quỹ âm**: API `/api/transactions/summary` trả về cờ `isNegative` nếu số dư < 0

5. **Chia team tự động**: API `/api/challenges/{id}/auto-divide-teams` tự động chia đều TeamA/TeamB dựa trên rank (zigzag)

### Troubleshooting

#### Lỗi Migration
```bash
# Xóa migration cũ
dotnet ef migrations remove

# Tạo lại migration
dotnet ef migrations add InitialCreate
dotnet ef database update
```

#### Lỗi Connection String
- Đảm bảo SQL Server đang chạy
- Kiểm tra tên server, username, password
- Với LocalDB: Cài đặt SQL Server Express LocalDB

#### Lỗi CORS
- Backend đã cấu hình CORS cho `http://localhost:5173` (Vue dev server)
- Nếu frontend chạy ở port khác, cập nhật CORS trong `Program.cs`

### License
Project này được tạo cho mục đích học tập - Môn Fullstack Development
