# Hướng Dẫn Cài Đặt & Khởi Tạo Dự Án (Setup Guide)

Tài liệu này hướng dẫn chi tiết các bước cài đặt môi trường, cấu hình cơ sở dữ liệu và khởi chạy dự án **The Drink VN** trên máy local của bạn.

---

## Yêu Cầu Hệ Thống (Prerequisites)

Trước khi bắt đầu, hãy đảm bảo máy tính của bạn đã cài đặt các công cụ sau:
1. **[.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)**
2. **[SQL Server](https://www.microsoft.com/sql-server/sql-server-downloads)** (bản Developer hoặc Express)
3. **[SQL Server Management Studio (SSMS)](https://learn.microsoft.com/sql/ssms/download-sql-server-management-studio-ssms)** hoặc công cụ quản lý SQL Server tương đương.

---

## Hướng Dẫn Các Bước Thực Hiện

### Bước 1: Cấu hình Connection String

1. Mở tệp [appsettings.json](ASPNET-DK24TTC8-trantrunghai-trunghai/src/BanCaPheNuocGiaiKhat/appsettings.json).
2. Kiểm tra chuỗi kết nối `DefaultConnection`. Đảm bảo rằng thông tin kết nối khớp với cấu hình SQL Server trên máy bạn, đặc biệt là tài khoản đăng nhập `sa` và mật khẩu `123`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=thedrinkvn_db;User Id=sa;Password=123;TrustServerCertificate=True;"
}
```

> [!IMPORTANT]
> Nếu SQL Server của bạn sử dụng Instance Name (ví dụ: `localhost\SQLEXPRESS`), hãy cập nhật thuộc tính `Server` tương ứng (ví dụ: `Server=localhost\\SQLEXPRESS`).

---

### Bước 2: Dọn dẹp Database cũ (Nếu có)

Nếu trước đó bạn đã chạy dự án và muốn cài đặt lại database mới hoàn toàn để tránh xung đột dữ liệu:
1. Mở **SSMS** và kết nối vào SQL Server local.
2. Tìm database `thedrinkvn_db`.
3. Click chuột phải chọn **Delete** -> Tick chọn **Close existing connections** -> Bấm **OK** để xóa database cũ.

---

### Bước 3: Tạo Database (Nếu chưa tồn tại)

Nếu cơ sở dữ liệu `thedrinkvn_db` chưa có trên SQL Server của bạn, bạn có thể chạy câu lệnh sau trong SSMS để tạo mới:

```sql
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'thedrinkvn_db')
BEGIN
    CREATE DATABASE thedrinkvn_db;
END
```
---

### Bước 4: Cập nhật cấu trúc cơ sở dữ liệu (Migrations)

Để tạo các bảng và thiết lập quan hệ trong cơ sở dữ liệu `thedrinkvn_db` vừa tạo:
1. Mở terminal, di chuyển vào thư mục dự án chứa mã nguồn:
   ```bash
   cd src/BanCaPheNuocGiaiKhat
   ```
2. Nếu máy của bạn chưa cài đặt công cụ Entity Framework Core CLI, hãy cài đặt bằng lệnh:
   ```bash
   dotnet tool install --global dotnet-ef
   ```
3. Chạy lệnh sau để áp dụng tất cả các bản migration hiện có vào cơ sở dữ liệu:
   ```bash
   dotnet ef database update
   ```

---

### Bước 5: Chạy ứng dụng & Khởi tạo dữ liệu mẫu (Seeding)

Sau khi cơ sở dữ liệu đã có đầy đủ cấu trúc bảng, chạy lệnh sau tại thư mục `src/BanCaPheNuocGiaiKhat` để khởi chạy dự án:

```bash
dotnet run
```

Khi dự án chạy thành công, log trên console sẽ hiển thị thông báo:
```text
Database migration và seed roles thành công.
[ProductSeeder] Khởi tạo dữ liệu danh mục và sản phẩm mẫu hoàn tất.
[IdentitySeeder] Khởi tạo dữ liệu người dùng mẫu hoàn tất.
```

---

## Kiểm Tra Dữ Liệu Sau Khi Seed

Sau khi ứng dụng khởi chạy thành công, cơ sở dữ liệu `thedrinkvn_db` sẽ được tạo mới với đầy đủ các bảng và dữ liệu sau:

### 1. Tài khoản đăng nhập mẫu (IdentitySeeder)

Hệ thống tự động thêm các tài khoản mẫu sau vào cơ sở dữ liệu:

| Họ và Tên | Email đăng nhập | Mật khẩu | Vai trò (Role) |
| :--- | :--- | :--- | :--- |
| **System Admin** | `admin@thedrinkvn.local` | `Admin@123` | Quản trị viên (Admin) |
| **Store Staff** | `staff@thedrinkvn.local` | `Staff@123` | Nhân viên (Staff) |
| **Nhân viên 2** | `trunghai.2102@gmail.com` | `Demo@123` | Nhân viên (Staff) |
| **Nhân viên 3** | `trunghai.pn@gmail.com` | `Demo@123` | Nhân viên (Staff) |
| **Trung hải** | `trunghai.21@gmail.com` | `Demo@123` | Khách hàng (Customer) |

### 2. Dữ liệu sản phẩm mẫu (ProductSeeder)

Hệ thống tự động thêm **4 danh mục chính** và **44 sản phẩm** chi tiết kèm theo hình ảnh, mô tả tương ứng:
- **Cà phê hạt** (19 sản phẩm)
- **Cà phê bột** (8 sản phẩm)
- **Dụng cụ pha chế** (12 sản phẩm)
- **Nước giải khát** (5 sản phẩm)

---

## Truy Cập Giao Diện Ứng Dụng

Ứng dụng của bạn sẽ hoạt động tại địa chỉ:
* **HTTP:** [http://localhost:5200](http://localhost:5200)
