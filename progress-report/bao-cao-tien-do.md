# BÁO CÁO TIẾN ĐỘ DỰ ÁN
## Website Bán Sản Phẩm Cà Phê & Nước Giải Khát — *The Drink VN*

> **Giảng viên hướng dẫn:** ThS. Lê Nguyên Thảo  
> **Công nghệ:** C# ASP.NET MVC · MySQL · The Drink VN  
> **Cập nhật lần cuối:** 16/06/2026

---

## Tổng Quan Tiến Độ

| # | Module / Hạng mục | Trạng thái | Tiến độ |
|---|---|---|---|
| 1 | Cơ sở lý thuyết | Hoàn thành | 100% |
| 2 | Phân tích yêu cầu hệ thống | Hoàn thành | 100% |
| 3 | Thiết kế kiến trúc hệ thống | Hoàn thành | 100% |
| 4 | Thiết kế Use Case | Hoàn thành | 100% |
| 5 | Thiết kế cơ sở dữ liệu | Hoàn thành | 100% |
| 6 | Module Xác thực (Auth) | Hoàn thành | 100% |
| 7 | Module Sản phẩm & Danh mục | Hoàn thành | 100% |
| 8 | Module Giỏ hàng & Đặt hàng | Hoàn thành | 100% |
| 9 | Module Thanh toán | Hoàn thành | 100% |
| 10 | Module Nhân viên | Hoàn thành | 100% |
| 11 | Module Admin / Quản trị | Hoàn thành | 100% |
| 12 | Module Thống kê doanh thu | Hoàn thành | 100% |
| 13 | Hoàn thiện báo cáo | Đang thực hiện | 80% |

> **Tiến độ tổng thể ước tính: ~95%**

---

## PHẦN I — ĐÃ HOÀN THÀNH

### 1. Cơ Sở Lý Thuyết
- [x] Ngôn ngữ lập trình C# — giới thiệu, tính năng, ưu/nhược điểm
- [x] Hệ quản trị CSDL MySQL — giới thiệu, tính năng, ưu/nhược điểm
- [x] Mô hình kiến trúc MVC (Model – View – Controller)

### 2. Phân Tích Yêu Cầu Hệ Thống
- [x] Mô tả bài toán nghiệp vụ
- [x] Mô hình hoạt động của cửa hàng
- [x] Yêu cầu chức năng — 3 nhóm người dùng (Khách hàng / Nhân viên / Admin)
- [x] Yêu cầu phi chức năng (bảo mật, hiệu năng, giao diện, đa thiết bị)

### 3. Thiết Kế Kiến Trúc Hệ Thống
- [x] Mô hình kiến trúc tổng thể (MVC + MySQL)

### 4. Thiết Kế Use Case
- [x] Lược đồ Use Case hệ thống đầy đủ (3 actor: Khách hàng, Nhân viên, Admin)

---

## PHẦN II — ĐÃ HOÀN THÀNH

### 5. Thiết Kế Cơ Sở Dữ Liệu 
- [x] Xác định các thực thể chính của hệ thống
- [x] Xác định quan hệ giữa các bảng
- [x] Hoàn thiện ERD / Diagram đầy đủ
- [x] Mô tả chi tiết từng bảng (tên cột, kiểu dữ liệu, ràng buộc)

> **Các bảng thực tế:**
> `Users` · `Roles` · `Categories` · `Products` · `ProductImages` · `CartItems` · `Orders` · `OrderItems` · `Invoices`

---

## PHẦN III — TIẾN ĐỘ THỰC HIỆN CÁC MODULE

### 6. Module Xác Thực (Authentication) *Hoàn thành*
- [x] Đăng ký tài khoản (họ tên, email, SĐT, username, mật khẩu)
- [x] Đăng nhập / Đăng xuất
- [x] Đổi mật khẩu / Quên mật khẩu
- [x] Phân quyền (Role: Customer / Staff / Admin)

---

### 7. Module Sản Phẩm & Danh Mục *Hoàn thành*
- [x] Trang danh sách sản phẩm (lọc theo danh mục, mức rang, vùng trồng, kiểu xay...)
- [x] Trang chi tiết sản phẩm (tên, hình ảnh, giá, mô tả, lượt xem, sản phẩm liên quan)
- [x] Tìm kiếm sản phẩm theo tên và mô tả
- [x] Sắp xếp sản phẩm theo giá tăng/giảm và độ phổ biến
- [x] Lọc sản phẩm (dựa trên category)
- [x] CRUD danh mục (Admin)
- [x] CRUD sản phẩm — thêm/sửa/xóa/tồn kho/upload nhiều hình ảnh (Admin)

---

### 8. Module Giỏ Hàng & Đặt Hàng *Hoàn thành*
- [x] Thêm sản phẩm vào giỏ hàng
- [x] Cập nhật số lượng sản phẩm trong giỏ hàng
- [x] Xóa sản phẩm khỏi giỏ hàng
- [x] Kiểm tra tồn kho thực tế trước khi đặt
- [x] Nhập thông tin giao nhận hàng (tên, SĐT, địa chỉ, ghi chú)
- [x] Xác nhận và gửi đơn hàng trực tuyến
- [x] Xem lịch sử danh sách đơn hàng (Khách hàng)
- [x] Theo dõi hành trình chi tiết đơn hàng (Khách hàng)

---

### 9. Module Thanh Toán *Hoàn thành*
- [x] Thanh toán khi nhận hàng (COD) cho đơn online
- [x] Thanh toán tiền mặt tại quầy (POS) cho đơn offline
- [x] Lưu lịch sử giao dịch (Tự động xuất Hóa đơn sau khi thanh toán)

---

### 10. Module Nhân Viên *Hoàn thành*
- [x] Xem danh sách đơn hàng online (lọc theo trạng thái)
- [x] Tiếp nhận và xử lý đơn hàng được phân công
- [x] Cập nhật trạng thái đơn hàng (pending -> processing -> shipping -> delivered -> cancelled)
- [x] Bán hàng tại quầy POS trực quan (chọn món, tính tiền thối)
- [x] Xuất hóa đơn giấy/điện tử cho khách
- [x] Kiểm tra tồn kho thời gian thực khi bán hàng

---

### 11. Module Admin / Quản Trị *Hoàn thành*
- [x] Dashboard tổng quan (Doanh thu, số đơn hàng, giá trị trung bình, sản phẩm bán ra)
- [x] Bộ lọc thống kê doanh thu linh hoạt (theo tháng/năm hoặc từ ngày - đến ngày)
- [x] Biểu đồ trực quan doanh thu theo ngày và Top 6 sản phẩm bán chạy nhất
- [x] Quản lý danh mục (thêm / sửa / xóa)
- [x] Quản lý sản phẩm (CRUD, tải ảnh động, soft-delete)
- [x] Quản lý người dùng (xem danh sách, thay đổi vai trò, khóa/mở khóa tài khoản)
- [x] Theo dõi và quản lý toàn bộ hóa đơn/đơn hàng hệ thống

---

### 12. Module Thống Kê Doanh Thu *Hoàn thành*
- [x] Thống kê doanh thu theo ngày
- [x] Thống kê doanh thu theo tháng / năm
- [x] Thống kê doanh thu theo khoảng thời gian tùy chọn
- [x] Báo cáo sản phẩm bán chạy nhất
- [x] Biểu đồ trực quan doanh thu & số đơn hàng phát sinh

---

### 13. Hoàn Thiện Báo Cáo *Đang làm*
- [x] Mở đầu (lý do, mục tiêu, đối tượng, phạm vi nghiên cứu)
- [x] Chương Cơ sở lý thuyết
- [x] Chương Hiện thực hóa — phân tích yêu cầu, use case, kiến trúc
- [x] Hoàn thiện phần Thiết kế CSDL (diagram + mô tả bảng)
- [x] Chương 4 — Kết quả (chụp ảnh màn hình giao diện thực tế và mô tả chức năng)
- [ ] Chương 5 — Kết luận và hướng phát triển
- [ ] Tóm tắt đồ án
- [ ] Mục lục tự động
- [ ] Danh mục hình ảnh & bảng biểu
- [ ] Danh mục tài liệu tham khảo

---

## Ghi Chú
- Tên thương hiệu: **The Drink VN**
- Stack: **C# ASP.NET MVC** + **MySQL**
- Đăng nhập bên thứ ba: **Google**, **Facebook**
- Tài liệu gốc: `BaoCaoDoAnCafeGiaiKhat.doc`
