# BÁO CÁO TIẾN ĐỘ DỰ ÁN
## Website Bán Sản Phẩm Cà Phê & Nước Giải Khát — *The Drink VN*

> **Giảng viên hướng dẫn:** ThS. Lê Nguyên Thảo  
> **Công nghệ:** C# ASP.NET MVC · MySQL · The Drink VN  
> **Cập nhật lần cuối:** 08/06/2026

---

## Tổng Quan Tiến Độ

| # | Module / Hạng mục | Trạng thái | Tiến độ |
|---|---|---|---|
| 1 | Cơ sở lý thuyết | Hoàn thành | 100% |
| 2 | Phân tích yêu cầu hệ thống | Hoàn thành | 100% |
| 3 | Thiết kế kiến trúc hệ thống | Hoàn thành | 100% |
| 4 | Thiết kế Use Case | Hoàn thành | 100% |
| 5 | Thiết kế cơ sở dữ liệu | Đang thực hiện | 100% |
| 6 | Module Xác thực (Auth) | Chưa bắt đầu | 0% |
| 7 | Module Sản phẩm & Danh mục | Chưa bắt đầu | 0% |
| 8 | Module Giỏ hàng & Đặt hàng | Chưa bắt đầu | 0% |
| 9 | Module Thanh toán | Chưa bắt đầu | 0% |
| 10 | Module Nhân viên | Chưa bắt đầu | 0% |
| 11 | Module Admin / Quản trị | Chưa bắt đầu | 0% |
| 12 | Module Thống kê doanh thu | Chưa bắt đầu | 0% |
| 13 | Hoàn thiện báo cáo | Đang thực hiện | 30% |

> **Tiến độ tổng thể ước tính: ~35%**

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

> **Các bảng dự kiến:**
> `Users` · `Roles` · `Categories` · `Products` · `ProductImages` · `Cart` · `CartItems` · `Orders` · `OrderDetails` · `Payments` · `Reviews` · `Employees`

---

## PHẦN III — ĐANG LÀM
### 6. Module Xác Thực (Authentication) *Đang làm*
- [ ] Đăng ký tài khoản (họ tên, email, SĐT, username, mật khẩu)
- [ ] Đăng nhập / Đăng xuất
- [ ] Đăng nhập bằng Google OAuth
- [ ] Đăng nhập bằng Facebook OAuth
- [ ] Đổi mật khẩu
- [ ] Phân quyền (Role: Customer / Staff / Admin)

---

### 7. Module Sản Phẩm & Danh Mục
- [ ] Trang danh sách sản phẩm (lọc theo danh mục: cà phê, trà sữa, nước ép, sinh tố...)
- [ ] Trang chi tiết sản phẩm (tên, hình ảnh, giá, mô tả, tình trạng)
- [ ] Tìm kiếm sản phẩm theo tên
- [ ] Lọc sản phẩm theo khoảng giá
- [ ] CRUD danh mục (Admin)
- [ ] CRUD sản phẩm — thêm/sửa/xóa/cập nhật tồn kho (Admin)

---

### 8. Module Giỏ Hàng & Đặt Hàng
- [ ] Thêm sản phẩm vào giỏ hàng
- [ ] Cập nhật số lượng sản phẩm trong giỏ
- [ ] Xóa sản phẩm khỏi giỏ
- [ ] Hiển thị tổng tiền thanh toán
- [ ] Nhập thông tin giao hàng
- [ ] Xác nhận và gửi đơn hàng
- [ ] Xem lịch sử đơn hàng (phía khách)

---

### 9. Module Thanh Toán
- [ ] Thanh toán khi nhận hàng (COD)
- [ ] Thanh toán trực tuyến (cổng thanh toán điện tử)
- [ ] Lưu lịch sử giao dịch

---

### 10. Module Nhân Viên
- [ ] Xem danh sách đơn hàng được phân công
- [ ] Kiểm tra thông tin khách hàng trên đơn
- [ ] Cập nhật trạng thái đơn hàng (đang xử lý / đang giao / hoàn thành / hủy)
- [ ] Xác nhận đơn hàng
- [ ] Xuất hóa đơn cho khách
- [ ] Hỗ trợ xử lý vấn đề đơn hàng

---

### 11. Module Admin / Quản Trị
- [ ] Dashboard tổng quan
- [ ] Quản lý danh mục (thêm / sửa / xóa)
- [ ] Quản lý sản phẩm (thêm / sửa / xóa / tồn kho)
- [ ] Quản lý khách hàng (xem danh sách, cập nhật, khóa/mở tài khoản)
- [ ] Quản lý nhân viên (thêm / sửa / phân quyền)
- [ ] Theo dõi toàn bộ đơn hàng hệ thống

---

### 12. Module Thống Kê Doanh Thu
- [ ] Thống kê doanh thu theo ngày
- [ ] Thống kê doanh thu theo tháng
- [ ] Thống kê doanh thu theo năm
- [ ] Báo cáo sản phẩm bán chạy
- [ ] Biểu đồ trực quan (chart)

---

### 14. Hoàn Thiện Báo Cáo
- [x] Mở đầu (lý do, mục tiêu, đối tượng, phạm vi nghiên cứu)
- [x] Chương Cơ sở lý thuyết
- [x] Chương Hiện thực hóa — phân tích yêu cầu, use case, kiến trúc
- [ ] Hoàn thiện phần Thiết kế CSDL (diagram + mô tả bảng)
- [ ] Chương 4 — Kết quả (ảnh chụp màn hình, mô tả từng chức năng)
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
