# BÁO CÁO CẬP NHẬT TIẾN ĐỘ CHI TIẾT
## Giai đoạn: 09/06/2026 – 16/06/2026
### Dự án: Website Bán Sản Phẩm Cà Phê & Nước Giải Khát — *The Drink VN*

---

## I. THÔNG TIN CHUNG
* **Thời gian thực hiện:** 09/06/2026 – 16/06/2026
* **Công nghệ sử dụng:** C# ASP.NET Core MVC, Entity Framework Core, MySQL, Bootstrap, Vanilla CSS/JS
* **Mục tiêu giai đoạn:** Xây dựng cấu trúc dự án cốt lõi, hoàn thiện cơ sở dữ liệu, phát triển toàn bộ các tính năng nghiệp vụ chính bao gồm: Giỏ hàng, Đặt hàng trực tuyến, Quản lý đơn hàng (nhân viên), Bán hàng tại quầy POS, Xuất hóa đơn, Quản lý sản phẩm (CRUD Admin), Quản lý người dùng (Admin) và Dashboard thống kê doanh thu.
* **Tiến độ tổng thể hệ thống:** Đạt **~85%** (Tăng vượt bậc từ mức ~35% của tuần trước).

---

## II. DANH SÁCH CÁC TÍNH NĂNG MỚI ĐÃ HOÀN THÀNH

### 1. Xác thực, Phân quyền & Quản lý Tài khoản (Auth Module)
* **Tính năng đã triển khai:**
  * **Đăng ký / Đăng nhập / Đăng xuất:** Quy trình đăng ký tài khoản mới cho khách hàng; Đăng nhập xác thực bằng Cookie Authentication; Đăng xuất an toàn.
  * **Đổi mật khẩu:** Cho phép người dùng đã đăng nhập tự đổi mật khẩu cá nhân.
  * **Phân quyền người dùng (Role-based Authorization):** Hệ thống phân quyền chặt chẽ theo 3 nhóm vai trò: `customer` (Khách hàng), `staff` (Nhân viên), và `admin` (Quản trị viên). Điều hướng người dùng tự động về trang chủ tương ứng sau khi đăng nhập thành công.
  * **Seeder tài khoản mặc định:** Tự động tạo sẵn tài khoản quản trị viên (`admin@thedrinkvn.local`) và nhân viên (`staff@thedrinkvn.local`) khi khởi chạy ứng dụng lần đầu.
  * **Tối ưu mã hóa bảo mật:** Tách riêng logic băm mật khẩu PBKDF2 thành tiện ích dùng chung `PasswordHasher`.
* **Các file mã nguồn liên quan:**
  * Bộ điều khiển: [AuthController.cs](file:///c:/Users/sangn/Documents/git/ASPNET-DK24TTC8-trantrunghai-trunghai/src/BanCaPheNuocGiaiKhat/Controllers/AuthController.cs)
  * Bộ băm mật khẩu: [PasswordHasher.cs](file:///c:/Users/sangn/Documents/git/ASPNET-DK24TTC8-trantrunghai-trunghai/src/BanCaPheNuocGiaiKhat/Utils/PasswordHasher.cs)
  * Khởi tạo dữ liệu mẫu: [IdentitySeeder.cs](file:///c:/Users/sangn/Documents/git/ASPNET-DK24TTC8-trantrunghai-trunghai/src/BanCaPheNuocGiaiKhat/Data/IdentitySeeder.cs)

### 2. Danh mục & Hiển thị Sản phẩm (Product Catalog)
* **Tính năng đã triển khai:**
  * **Bộ lọc sản phẩm đa năng:** Cho phép lọc sản phẩm linh hoạt theo nhiều tiêu chí đồng thời: danh mục (Cà phê, Trà sữa, Nước ép, Sinh tố...), mức độ rang (Roast Level), vùng trồng hạt cà phê (Region), và kiểu xay hạt (Grind Type).
  * **Tìm kiếm & Sắp xếp:** Tìm kiếm sản phẩm theo từ khóa (tên hoặc mô tả ngắn); Sắp xếp theo giá tăng dần/giảm dần, hoặc theo độ phổ biến (lượt xem).
  * **Phân trang:** Chia trang danh sách sản phẩm khoa học để tối ưu hiệu năng tải trang.
  * **Trang chi tiết sản phẩm:** Hiển thị thông số chi tiết sản phẩm, bộ sưu tập hình ảnh đính kèm, tăng số lượt xem tự động (`ViewCount`), hiển thị đề xuất các sản phẩm liên quan cùng danh mục.
* **Các file mã nguồn liên quan:**
  * Bộ điều khiển: [SanPhamController.cs](file:///c:/Users/sangn/Documents/git/ASPNET-DK24TTC8-trantrunghai-trunghai/src/BanCaPheNuocGiaiKhat/Controllers/SanPhamController.cs) (phần client)
  * Models hiển thị: [ProductViewModels.cs](file:///c:/Users/sangn/Documents/git/ASPNET-DK24TTC8-trantrunghai-trunghai/src/BanCaPheNuocGiaiKhat/Models/ProductViewModels.cs)

### 3. Giỏ hàng & Đặt hàng trực tuyến (Cart & Online Checkout)
* **Tính năng đã triển khai:**
  * **Quản lý giỏ hàng trực quan (Khách hàng):** Cho phép khách hàng thêm sản phẩm vào giỏ, cập nhật số lượng, xóa sản phẩm trực tiếp từ giao diện giỏ hàng.
  * **Ràng buộc tồn kho thực tế:** Hệ thống tự động kiểm tra số lượng tồn kho của sản phẩm ở tất cả các khâu (thêm vào giỏ, cập nhật giỏ, trước khi đặt hàng) để đảm bảo không đặt vượt quá số lượng trong kho.
  * **Đặt hàng trực tuyến (Online Order):** Khách hàng điền thông tin người nhận (Họ tên, SĐT, Địa chỉ giao hàng, Ghi chú) và tiến hành đặt đơn hàng ở trạng thái chờ duyệt (`pending`). Tự động làm trống giỏ hàng sau khi đặt thành công.
  * **Lịch sử & Theo dõi đơn hàng:** Trang danh sách đơn hàng cá nhân hiển thị trạng thái xử lý và trạng thái thanh toán. Xem chi tiết hành trình vận chuyển của từng đơn hàng.
* **Các file mã nguồn liên quan:**
  * Quản lý giỏ hàng: [GioHangController.cs](file:///c:/Users/sangn/Documents/git/ASPNET-DK24TTC8-trantrunghai-trunghai/src/BanCaPheNuocGiaiKhat/Controllers/GioHangController.cs)
  * Quy trình đặt hàng: [DatHangController.cs](file:///c:/Users/sangn/Documents/git/ASPNET-DK24TTC8-trantrunghai-trunghai/src/BanCaPheNuocGiaiKhat/Controllers/DatHangController.cs)

### 4. Quản lý Đơn hàng Online dành cho Nhân viên (Staff Order Management)
* **Tính năng đã triển khai:**
  * **Danh sách đơn hàng Online:** Giao diện cho nhân viên xem và lọc đơn hàng theo các trạng thái: Chờ xử lý (`pending`), Đang làm (`processing`), Đang giao (`shipping`), Đã giao (`delivered`), Đã hủy (`cancelled`).
  * **Xử lý trạng thái & Phân công:** Nhân viên bấm tiếp nhận đơn hàng (hệ thống tự động ghi nhận nhân viên xử lý đơn hàng đó), cập nhật tiến trình đơn hàng từ khi làm nước đến khi giao cho shipper và hoàn tất.
  * **Xác nhận thanh toán:** Khi giao hàng thành công, nhân viên xác nhận thanh toán để hoàn tất đơn hàng trực tuyến.
* **Các file mã nguồn liên quan:**
  * Bộ điều khiển: [DonHangOnlineController.cs](file:///c:/Users/sangn/Documents/git/ASPNET-DK24TTC8-trantrunghai-trunghai/src/BanCaPheNuocGiaiKhat/Controllers/DonHangOnlineController.cs)
  * Models giao tiếp: [DonHangOnlineViewModels.cs](file:///c:/Users/sangn/Documents/git/ASPNET-DK24TTC8-trantrunghai-trunghai/src/BanCaPheNuocGiaiKhat/Models/DonHangOnlineViewModels.cs)

### 5. Nghiệp vụ Bán hàng tại quầy POS & Hóa đơn (POS & Invoicing - Staff)
* **Tính năng đã triển khai:**
  * **Giao diện bán hàng nhanh (POS):** Thiết kế trực quan cho nhân viên bán hàng tại quầy. Hiển thị danh sách sản phẩm kèm ảnh và giá, nhân viên chỉ cần nhấn chọn sản phẩm và tăng/giảm số lượng.
  * **Kiểm tra tồn kho tức thì:** Cảnh báo ngay lập tức nếu số lượng sản phẩm chọn vượt quá số lượng tồn kho thực tế của cửa hàng.
  * **Thanh toán & Tính tiền thối:** Nhập số tiền khách đưa (hỗ trợ nhập theo đơn vị VND thông dụng), hệ thống tự động tính toán số tiền cần thối lại chính xác cho khách hàng.
  * **Xuất hóa đơn & Quản lý Hóa đơn:** Tạo hóa đơn (`Invoice`) lưu trữ thông tin thanh toán vào cơ sở dữ liệu. Xuất giao diện hóa đơn đẹp mắt, chi tiết (mã hóa đơn định dạng `HD-XXXXXX`, thời gian xuất, danh sách món, tiền khách đưa, tiền thừa).
  * **Danh sách hóa đơn:** Hỗ trợ nhân viên và admin tìm kiếm, theo dõi và quản lý/xóa tất cả các hóa đơn đã xuất.
* **Các file mã nguồn liên quan:**
  * Bán hàng POS: [BanHangController.cs](file:///c:/Users/sangn/Documents/git/ASPNET-DK24TTC8-trantrunghai-trunghai/src/BanCaPheNuocGiaiKhat/Controllers/BanHangController.cs)
  * Quản lý hóa đơn: [HoaDonController.cs](file:///c:/Users/sangn/Documents/git/ASPNET-DK24TTC8-trantrunghai-trunghai/src/BanCaPheNuocGiaiKhat/Controllers/HoaDonController.cs)
  * Models POS: [BanHangViewModels.cs](file:///c:/Users/sangn/Documents/git/ASPNET-DK24TTC8-trantrunghai-trunghai/src/BanCaPheNuocGiaiKhat/Models/BanHangViewModels.cs)
  * Models hóa đơn: [HoaDonViewModels.cs](file:///c:/Users/sangn/Documents/git/ASPNET-DK24TTC8-trantrunghai-trunghai/src/BanCaPheNuocGiaiKhat/Models/HoaDonViewModels.cs)

### 6. Quản trị nâng cao dành cho Admin (Admin Dashboard & CRUD)
* **Tính năng đã triển khai:**
  * **Trang chủ Admin (Dashboard):** Thống kê các số liệu kinh doanh cốt lõi trực quan: Tổng doanh thu bán hàng, Tổng số đơn hàng thành công, Giá trị đơn hàng trung bình, Tổng số lượng sản phẩm bán ra.
  * **Thống kê theo thời gian linh hoạt:** Cho phép xem thống kê theo Tháng/Năm lựa chọn, hoặc lọc tùy biến theo khoảng thời gian "Từ ngày – Đến ngày".
  * **Biểu đồ doanh thu & Top bán chạy:** Hiển thị dữ liệu doanh thu biến động theo ngày, liệt kê danh sách 6 sản phẩm bán chạy nhất cùng doanh thu cụ thể của từng sản phẩm, và danh sách 5 hóa đơn mới phát sinh gần nhất.
  * **Quản lý Sản phẩm (CRUD nâng cao):**
    * Xem danh sách sản phẩm quản trị (tên, danh mục, giá, tồn kho, trạng thái, ảnh đại diện).
    * Thêm mới sản phẩm đi kèm chức năng upload nhiều hình ảnh đồng thời vào thư mục vật lý `wwwroot/uploads/products`.
    * Chỉnh sửa thông tin sản phẩm, cập nhật ảnh mới, xóa ảnh cũ đã chọn, tự động tính toán lại ảnh đại diện chính từ ảnh đầu tiên còn lại.
    * Xóa sản phẩm (hệ thống sử dụng cơ chế xóa mềm - soft-delete bằng cách cập nhật trạng thái `Status = "deleted"` để bảo toàn tính toàn vẹn của dữ liệu hóa đơn cũ).
  * **Quản lý Người dùng (User Management):**
    * Danh sách toàn bộ tài khoản trong hệ thống, tìm kiếm nhanh theo Tên/Email/SĐT, lọc theo vai trò (Role).
    * Thay đổi quyền hạn (Role) trực tiếp của người dùng khác (khóa chặn tự đổi vai trò của chính mình để tránh lỗi logic hệ thống).
    * Khóa / Mở khóa tài khoản người dùng nhanh chóng (cập nhật trạng thái `Status = "active" / "locked"`).
* **Các file mã nguồn liên quan:**
  * Quản trị sản phẩm: [SanPhamController.cs](file:///c:/Users/sangn/Documents/git/ASPNET-DK24TTC8-trantrunghai-trunghai/src/BanCaPheNuocGiaiKhat/Controllers/SanPhamController.cs) (phần Admin CRUD)
  * Quản trị người dùng: [NguoiDungController.cs](file:///c:/Users/sangn/Documents/git/ASPNET-DK24TTC8-trantrunghai-trunghai/src/BanCaPheNuocGiaiKhat/Controllers/NguoiDungController.cs)
  * Models Dashboard: [AdminDashboardViewModels.cs](file:///c:/Users/sangn/Documents/git/ASPNET-DK24TTC8-trantrunghai-trunghai/src/BanCaPheNuocGiaiKhat/Models/AdminDashboardViewModels.cs)
  * Giao diện Admin Layout chung: [Shared/_AdminLayout.cshtml](file:///c:/Users/sangn/Documents/git/ASPNET-DK24TTC8-trantrunghai-trunghai/src/BanCaPheNuocGiaiKhat/Views/Shared/_AdminLayout.cshtml)

---

## III. CẬP NHẬT CẤU TRÚC CƠ SỞ DỮ LIỆU (DATABASE SCHEMA)
Trong giai đoạn này, cơ sở dữ liệu đã được mở rộng mạnh mẽ qua các bản Migration để hỗ trợ giỏ hàng, đặt hàng và hóa đơn:
1. **Bảng `products` & `product_images`:** Hỗ trợ lưu thông tin chi tiết hạt cà phê (RoastLevel, Region, GrindType) và nhiều hình ảnh của sản phẩm.
2. **Bảng `cart_items`:** Lưu trữ giỏ hàng tạm thời của khách hàng.
3. **Bảng `orders` & `order_items`:**
   * Lưu trữ chi tiết đơn hàng (loại đơn hàng: `online` / `offline`, trạng thái: `pending`, `processing`, `shipping`, `delivered`, `cancelled`).
   * Lưu vết giá bán tại thời điểm mua hàng (để tránh thay đổi giá sản phẩm sau này ảnh hưởng đến doanh thu cũ).
4. **Bảng `invoices`:** Lưu trữ thông tin thanh toán cuối cùng của đơn hàng (tiền khách đưa, tiền thừa, mã hóa đơn).

---

## IV. BẢNG SO SÁNH TIẾN ĐỘ THỰC TẾ

| STT | Module chức năng | Tiến độ trước (08/06) | Tiến độ hiện tại (16/06) | Trạng thái chi tiết |
|:---:|---|:---:|:---:|---|
| 1 | Cơ sở dữ liệu & Entity | 90% | **100%** | Đã hoàn thành sơ đồ thực thể ERD, đồng bộ qua migrations và seed dữ liệu mẫu đầy đủ. |
| 2 | Module Xác thực (Auth) | 50% | **85%** | Đã hoàn thành Đăng nhập/Đăng ký/Đăng xuất/Đổi mật khẩu và phân vai trò. Còn thiếu liên kết Google/Facebook OAuth. |
| 3 | Module Sản phẩm & Danh mục | 40% | **90%** | Đã hoàn thành bộ lọc nâng cao, tìm kiếm, sắp xếp và trang chi tiết. Đã hoàn thành CRUD sản phẩm Admin. Còn thiếu CRUD danh mục trực quan. |
| 4 | Module Giỏ hàng & Đặt hàng | 0% | **100%** | Hoàn thành toàn bộ quy trình thêm vào giỏ, checkout online, kiểm tra tồn kho và quản lý lịch sử đơn hàng. |
| 5 | Module Thanh toán | 0% | **100%** | Hoàn thành thanh toán COD (Online) và tiền mặt (POS), tự động lưu vết giao dịch. |
| 6 | Module Nhân viên (Staff) | 0% | **100%** | Hoàn thành giao diện POS bán hàng tại quầy nhanh và quản lý, cập nhật trạng thái đơn hàng online. |
| 7 | Module Admin / Quản trị | 0% | **90%** | Hoàn thành Dashboard trực quan, quản lý sản phẩm (CRUD + upload ảnh) và quản lý người dùng (thay đổi vai trò, khóa tài khoản). |
| 8 | Module Thống kê doanh thu | 0% | **100%** | Hoàn thành tính toán doanh thu, biểu đồ doanh thu theo ngày, thống kê sản phẩm bán chạy theo thời gian tùy chọn. |
| 9 | Tài liệu / Báo cáo đồ án | 30% | **60%** | Đã viết xong Chương 1, 2, 3 và thiết kế CSDL. Đang cập nhật hình ảnh giao diện thực tế vào Chương 4. |

---

## V. KẾ HOẠCH CHO GIAI ĐOẠN TIẾP THEO (17/06 – 24/06)
1. **Kiểm thử hệ thống (System Testing):**
   * Thực hiện kiểm thử tích hợp (Integration Testing) giữa các module để phát hiện các lỗi logic tiềm ẩn.
   * Kiểm tra độ ổn định khi nhiều người dùng thao tác mua hàng cùng lúc (Concurrency test đối với trường tồn kho `StockQty`).
2. **Hoàn thiện các chức năng phụ:**
   * Nghiên cứu tích hợp Google OAuth cho việc đăng ký/đăng nhập nhanh (nếu thời gian cho phép).
   * Thêm giao diện quản lý CRUD Danh mục sản phẩm trực quan cho Admin.
3. **Hoàn tất Báo cáo Đồ án:**
   * Chụp ảnh màn hình giao diện thực tế của toàn bộ các tính năng đã phát triển để đưa vào Chương 4 (Kết quả thực hiện).
   * Hoàn thiện Chương 5 (Kết luận và Hướng phát triển).
   * Rà soát mục lục, tài liệu tham khảo và định dạng tài liệu theo chuẩn đồ án tốt nghiệp.
