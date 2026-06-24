# HƯỚNG DẪN CÀI ĐẶT VÀ CHẠY DỰ ÁN

Tài liệu này hướng dẫn chi tiết cách thiết lập môi trường và chạy dự án website **The Drink VN** (ASP.NET Core MVC).

## 1. Yêu Cầu Hệ Thống
Để chạy được project, máy tính của bạn cần cài đặt các phần mềm sau:
- **.NET 8 SDK**: [Tải tại đây](https://dotnet.microsoft.com/download/dotnet/8.0)
- **SQL Server**: Có thể dùng SQL Server Express hoặc Developer Edition.
- **SQL Server Management Studio (SSMS)** (Khuyên dùng) hoặc Azure Data Studio để quản lý Database.

## 2. Cấu Hình Cơ Sở Dữ Liệu
Mặc định, ứng dụng sử dụng chuỗi kết nối (Connection String) trỏ tới SQL Server local với tài khoản `sa`.
Bạn hãy mở file `src/BanCaPheNuocGiaiKhat/appsettings.json` và kiểm tra mục `ConnectionStrings`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=thedrinkvn_db;User Id=sa;Password=123456;TrustServerCertificate=True;"
}
```
*Lưu ý:* Hãy đổi `User Id` và `Password` cho khớp với SQL Server trên máy của bạn.

## 3. Khởi Chạy Dự Án

Mở Terminal (hoặc Command Prompt, PowerShell) tại thư mục chứa source code và thực hiện theo các bước sau:

### Bước 3.1: Di chuyển vào thư mục mã nguồn
```bash
cd src/BanCaPheNuocGiaiKhat
```

### Bước 3.2: Chạy dự án
Hệ thống đã được thiết lập tự động tạo Database (`EnsureCreated`) và tự động chạy Migrations cũng như Seed data. Bạn chỉ cần chạy lệnh:
```bash
dotnet run
```

Trong lần chạy đầu tiên, EF Core sẽ mất vài giây để:
1. Tạo Database `thedrinkvn_db`.
2. Áp dụng toàn bộ các cấu trúc bảng (Migrations).
3. Thêm (seed) dữ liệu mẫu bao gồm:
   - Các Role (admin, staff, customer)
   - Các tài khoản mặc định
   - Danh sách danh mục và hàng chục sản phẩm có sẵn (giá định dạng tiền VNĐ).

### Bước 3.3: Truy cập website
Mở trình duyệt và truy cập vào địa chỉ:
=> **http://localhost:5200** (hoặc port được báo trên Terminal).


## 4. Xử Lý Sự Cố Thường Gặp
**Lỗi: Cannot drop database "thedrinkvn_db" because it is currently in use**
- Nguyên nhân: Bạn đang mở kết nối đến DB này (ví dụ đang dùng SSMS) hoặc ứng dụng đang chạy ngầm.
- Khắc phục: Mở SSMS, mở một New Query và chạy lệnh sau để ép đóng kết nối và xóa DB:
```sql
USE master;
GO
ALTER DATABASE thedrinkvn_db SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
GO
DROP DATABASE thedrinkvn_db;
GO
```
Sau đó quay lại Terminal chạy `dotnet run`.
