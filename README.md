# The Drink VN — Website Bán Cà Phê & Nước Giải Khát

> **Đồ án môn học** · Chuyên đề ASP.NET
> **Giảng viên hướng dẫn:** · TS. Đoàn Phước Miền  
> **Thương hiệu:** The Drink VN

---

## Mục Lục

- [Giới thiệu](#-giới-thiệu)
- [Công nghệ sử dụng](#-công-nghệ-sử-dụng)
- [Kiến trúc hệ thống](#-kiến-trúc-hệ-thống)
- [Cấu trúc dự án](#-cấu-trúc-dự-án)
- [Thiết kế cơ sở dữ liệu](#-thiết-kế-cơ-sở-dữ-liệu)
- [Hướng dẫn cài đặt & chạy](#-hướng-dẫn-cài-đặt--chạy)
- [Tiến độ thực hiện](#-tiến-độ-thực-hiện)
- [Tính năng chi tiết](#-tính-năng-chi-tiết)
- [Nhật ký phát triển](#-nhật-ký-phát-triển)
- [Tài liệu tham khảo](#-tài-liệu-tham-khảo)

---

## Giới Thiệu

**The Drink VN** là website thương mại điện tử chuyên bán các sản phẩm **cà phê** và **nước giải khát**. Hệ thống được xây dựng nhằm phục vụ 3 nhóm người dùng:

| Vai trò | Mô tả |
|---|---|
| **Khách hàng** | Duyệt sản phẩm, đặt hàng, thanh toán, đánh giá |
| **Nhân viên** | Quản lý đơn hàng, xử lý giao hàng, xuất hóa đơn |
| **Admin** | Quản trị toàn bộ hệ thống (sản phẩm, danh mục, người dùng, thống kê) |

### Mục tiêu

- Xây dựng website bán hàng trực tuyến hoàn chỉnh cho quán cà phê & nước giải khát
- Áp dụng mô hình **MVC** (Model – View – Controller) để phân tách rõ ràng các lớp logic
- Hỗ trợ đăng nhập đa phương thức (tài khoản, Google OAuth, Facebook OAuth)
- Quản lý giỏ hàng, đặt hàng, thanh toán trực tuyến
- Dashboard thống kê doanh thu trực quan cho Admin

---

## Công Nghệ Sử Dụng

| Thành phần | Công nghệ | Phiên bản |
|---|---|---|
| **Ngôn ngữ** | C# | .NET 8.0 |
| **Framework** | ASP.NET Core MVC | 8.0 |
| **Cơ sở dữ liệu** | SQL Server| 2025 |
| **ORM / Data Access** | EF SQL Server | 8.x |
| **Front-end** | Razor Views + Bootstrap | 5.x |
| **IDE** | Visual Studio / VS Code | — |

---

## Kiến Trúc Hệ Thống

Dự án áp dụng mô hình **MVC (Model – View – Controller)**:

```
┌──────────────────────────────────────────────────────┐
│                     Client (Browser)                 │
└────────────────────────┬─────────────────────────────┘
                         │ HTTP Request
                         ▼
┌──────────────────────────────────────────────────────┐
│              ASP.NET Core MVC Application            │
│  ┌─────────────┐  ┌──────────────┐  ┌─────────────┐  │
│  │   Views     │  │  Controllers │  │   Models    │  │
│  │  (Razor)    │◄─│  (C#)        │──│  (C#)       │  │
│  │  + Bootstrap│  │              │  │             │  │
│  └─────────────┘  └──────┬───────┘  └──────┬──────┘  │
└──────────────────────────┼─────────────────┼─────────┘
                           │                 │
                           ▼                 ▼
                 ┌──────────────────────────────────┐
                 │        SQL Server 2025           │
                 │        Database: thedrinkvn_db   │
                 └──────────────────────────────────┘
```

- **Model**: Định nghĩa các thực thể dữ liệu và logic nghiệp vụ
- **View**: Giao diện người dùng bằng Razor + Bootstrap
- **Controller**: Xử lý request, điều hướng giữa Model và View

---

## Cấu Trúc Dự Án

```
Cafe_Website/
├── README.md                          # Tài liệu dự án (file này)
├── .gitignore                         # Danh sách file bỏ qua khi commit
│
├── src/                               # === MÃ NGUỒN ===
│   ├── BanCaPheNuocGiaiKhat.slnx      # Solution file
│   │
│   └── BanCaPheNuocGiaiKhat/          # Project chính (ASP.NET MVC)
│       ├── Program.cs                 # Entry point — cấu hình DI, middleware, routing
│       ├── BanCaPheNuocGiaiKhat.csproj # File cấu hình project (.NET 8)
│       ├── appsettings.json           # Connection string, cấu hình app
│       │
│       ├── Controllers/               # Các controller xử lý request
│       │   └── HomeController.cs      # Controller trang chủ
│       │
│       ├── Models/                    # Các model / entity
│       │   └── ErrorViewModel.cs      # Model xử lý lỗi
│       │
│       ├── Views/                     # Giao diện Razor
│       │   ├── Home/
│       │   │   ├── Index.cshtml       # Trang chủ
│       │   │   └── Privacy.cshtml     # Trang chính sách
│       │   ├── Shared/
│       │   │   ├── _Layout.cshtml     # Layout chung (navbar, footer)
│       │   │   ├── _Layout.cshtml.css # CSS cho layout
│       │   │   └── Error.cshtml       # Trang lỗi
│       │   ├── _ViewImports.cshtml    # Import TagHelpers
│       │   └── _ViewStart.cshtml      # Layout mặc định
│       │
│       └── wwwroot/                   # File tĩnh (CSS, JS, hình ảnh)
│           ├── css/
│           ├── js/
│           └── lib/                   # Thư viện (Bootstrap, jQuery)
│
├── thesis/                            # === TÀI LIỆU BÁO CÁO ===
│   └── doc/
│       └── BaoCaoDoAnCafeGiaiKhat.doc # Báo cáo đồ án (Word)
│
└── progress-report/                   # === BÁO CÁO TIẾN ĐỘ ===
    └── bao-cao-tien-do.md             # Tiến độ chi tiết từng module
```

---

## Thiết Kế Cơ Sở Dữ Liệu

### Danh sách các bảng

| # | Bảng | Mô tả |
|---|---|---|
| 1 | `roles` | Vai trò người dùng: admin, staff, customer |
| 2 | `users` | Người dùng hệ thống, gồm quản trị viên, nhân viên và khách hàng |
| 3 | `categories` | Danh mục sản phẩm đồ uống, hỗ trợ phân cấp bằng `parent_id` |
| 4 | `products` | Sản phẩm đồ uống, giá bán, mô tả, tồn kho và trạng thái |
| 5 | `product_images` | Thư viện hình ảnh sản phẩm, hỗ trợ nhiều ảnh cho mỗi sản phẩm |
| 6 | `payment_methods` | Danh mục phương thức thanh toán: COD, VNPay, MoMo, ZaloPay |
| 7 | `shipping_addresses` | Địa chỉ giao hàng đã lưu của khách hàng |
| 8 | `orders` | Đơn hàng, thông tin giao hàng, tổng tiền, thanh toán và trạng thái xử lý |
| 9 | `order_items` | Chi tiết sản phẩm trong đơn hàng, lưu snapshot thông tin lúc đặt |
| 10 | `cart_items` | Giỏ hàng tạm thời cho khách đăng nhập hoặc khách vãng lai |
| 11 | `vouchers` | Mã giảm giá và chương trình khuyến mãi |
| 12 | `voucher_usage` | Lịch sử áp dụng mã giảm giá cho đơn hàng |
| 13 | `invoices` | Hóa đơn bán hàng sau khi đơn hàng giao thành công |

> ERD (Entity Relationship Diagram) chi tiết xem tại file báo cáo: [`thesis/doc/BaoCaoDoAnCafeGiaiKhat.doc`](thesis/doc/BaoCaoDoAnCafeGiaiKhat.doc)
---

## Hướng Dẫn Cài Đặt & Chạy

### Yêu cầu hệ thống

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [MySQL8.4.6](https://downloads.mysql.com/archives/get/p/23/file/mysql-8.4.6-winx64.msi)
### Bước 1: Clone repository

```bash
git clone https://github.com/trantrunghai2102/ASPNET-DK24TTC8-trantrunghai-trunghai
cd ASPNET-DK24TTC8-trantrunghai-trunghai
```

### Bước 2: Chạy ứng dụng

```bash
cd BanCaPheNuocGiaiKhat
dotnet run
``
Ứng dụng sẽ chạy tại: **https://localhost:5200** 
```
---

## Tiến Độ Thực Hiện

> **Cập nhật lần cuối:** 19/06/2026  
> **Tiến độ tổng thể:** ~95%

| # | Module / Hạng mục | Trạng thái | Tiến độ |
|---|---|---|---|
| 1 | Cơ sở lý thuyết | Hoàn thành | 100% |
| 2 | Phân tích yêu cầu hệ thống | Hoàn thành | 100% |
| 3 | Thiết kế kiến trúc hệ thống | Hoàn thành | 100% |
| 4 | Thiết kế Use Case | Hoàn thành | 100% |
| 5 | Thiết kế cơ sở dữ liệu | Hoàn thành | 100% |
| 6 | Module Xác thực (Auth)| **Hoàn thành** | **100%** |
| 7 | Module Sản phẩm & Danh mục | Hoàn thành | 100% |
| 8 | Module Giỏ hàng & Đặt hàng | Hoàn thành | 100% |
| 9 | Module Thanh toán | Hoàn thành | 100% |
| 10 | Module Nhân viên | Hoàn thành | 100% |
| 11 | Module Admin / Quản trị | Hoàn thành | 100% |
| 12 | Module Thống kê doanh thu | Hoàn thành | 100% |
| 13 | Hoàn thiện báo cáo | Đang thực hiện | 80% |

---

## Tính Năng Chi Tiết

### Đã hoàn thành

#### 1. Cơ sở lý thuyết
- Ngôn ngữ lập trình C# — giới thiệu, tính năng, ưu/nhược điểm
- Hệ quản trị CSDL MySQL — giới thiệu, tính năng, ưu/nhược điểm
- Mô hình kiến trúc MVC (Model – View – Controller)

#### 2. Phân tích yêu cầu hệ thống
- Mô tả bài toán nghiệp vụ
- Mô hình hoạt động của cửa hàng
- Yêu cầu chức năng — 3 nhóm người dùng (Khách hàng / Nhân viên / Admin)
- Yêu cầu phi chức năng (bảo mật, hiệu năng, giao diện, đa thiết bị)

#### 3. Thiết kế kiến trúc hệ thống
- Mô hình kiến trúc tổng thể (ASP.NET MVC + MySQL)

#### 4. Thiết kế Use Case
- Lược đồ Use Case hệ thống đầy đủ (3 actor: Khách hàng, Nhân viên, Quản trị viên)

#### 5. Thiết kế cơ sở dữ liệu
- Xác định các thực thể chính của hệ thống
- Xác định quan hệ giữa các bảng
- Hoàn thiện ERD / Diagram đầy đủ
- Mô tả chi tiết từng bảng (tên cột, kiểu dữ liệu, ràng buộc)

#### 6. Khởi tạo dự án
- Tạo project ASP.NET Core MVC (.NET 8)
- Cấu hình kết nối MySQL (MySql.Data ADO.NET)
- Layout cơ bản với Bootstrap (Navbar, Footer)

---

### Đang thực hiện — Module Đăng Ký / Đăng Nhập (Auth)

> _Bắt đầu: 08/06/2026_

- [x] Đăng ký tài khoản (họ tên, email, SĐT, username, mật khẩu)
- [x] Đăng nhập / Đăng xuất
- [ ] Đăng nhập bằng Google OAuth
- [ ] Đăng nhập bằng Facebook OAuth
- [ ] Đổi mật khẩu
- [x] Phân quyền (Role: Customer / Staff / Admin)

---

### Chưa thực hiện

<details>
<summary><b>7. Module Sản Phẩm & Danh Mục</b></summary>

- [ ] Trang danh sách sản phẩm (lọc theo danh mục: cà phê, trà sữa, nước ép, sinh tố…)
- [ ] Trang chi tiết sản phẩm (tên, hình ảnh, giá, mô tả, tình trạng)
- [ ] Tìm kiếm sản phẩm theo tên
- [ ] Lọc sản phẩm theo khoảng giá
- [ ] CRUD danh mục (Admin)
- [ ] CRUD sản phẩm — thêm/sửa/xóa/cập nhật tồn kho (Admin)

</details>

<details>
<summary><b>8. Module Giỏ Hàng & Đặt Hàng</b></summary>

- [ ] Thêm sản phẩm vào giỏ hàng
- [ ] Cập nhật số lượng sản phẩm trong giỏ
- [ ] Xóa sản phẩm khỏi giỏ
- [ ] Hiển thị tổng tiền thanh toán
- [ ] Nhập thông tin giao hàng
- [ ] Xác nhận và gửi đơn hàng
- [ ] Xem lịch sử đơn hàng (phía khách)

</details>

<details>
<summary><b>9. Module Thanh Toán</b></summary>

- [ ] Thanh toán khi nhận hàng (COD)
- [ ] Thanh toán trực tuyến (cổng thanh toán điện tử)
- [ ] Lưu lịch sử giao dịch

</details>

<details>
<summary><b>10. Module Nhân Viên</b></summary>

- [ ] Xem danh sách đơn hàng được phân công
- [ ] Kiểm tra thông tin khách hàng trên đơn
- [ ] Cập nhật trạng thái đơn hàng (đang xử lý / đang giao / hoàn thành / hủy)
- [ ] Xác nhận đơn hàng
- [ ] Xuất hóa đơn cho khách
- [ ] Hỗ trợ xử lý vấn đề đơn hàng

</details>

<details>
<summary><b>11. Module Admin / Quản Trị</b></summary>

- [ ] Dashboard tổng quan
- [ ] Quản lý danh mục (thêm / sửa / xóa)
- [ ] Quản lý sản phẩm (thêm / sửa / xóa / tồn kho)
- [ ] Quản lý khách hàng (xem danh sách, cập nhật, khóa/mở tài khoản)
- [ ] Quản lý nhân viên (thêm / sửa / phân quyền)
- [ ] Theo dõi toàn bộ đơn hàng hệ thống

</details>

<details>
<summary><b>12. Module Thống Kê Doanh Thu</b></summary>

- [ ] Thống kê doanh thu theo ngày
- [ ] Thống kê doanh thu theo tháng
- [ ] Thống kê doanh thu theo năm
- [ ] Báo cáo sản phẩm bán chạy
- [ ] Biểu đồ trực quan (chart)

</details>

---

## Nhật Ký Phát Triển

| Ngày | Nội dung |
|---|---|
| 04/06/2026 | Hoàn thiện Cơ sở lý thuyết, phân tích yêu cầu hệ thống, thiết kế kiến trúc hệ thống, thiết kế Use Case, thiết kế cơ sở dữ liệu  |
| 08/06/2026 | Hoàn thiện tài liệu (CSDL, Use Case), cập nhật báo cáo, Hoàn thành module đăng ký/đăng nhập |
| 09/06/2026 - 15/06/2026 | Phát triển và hoàn thiện các module cốt lõi: Quản lý Sản phẩm & Danh mục, Giỏ hàng, Đặt hàng, Thanh toán, Admin Dashboard, Thống kê doanh thu và phân hệ Nhân viên. |
| 16/06/2026 - 21/06/2026 | Giai đoạn Refactor: Đồng bộ giao diện tiếng Việt, chuẩn hóa định dạng tiền tệ VNĐ (từ 25.00 sang 250,000), hoàn thiện toàn bộ tính năng Admin (Thống kê, Quản lý), Staff (Bán hàng POS, Quản lý đơn), Customer (Đặt hàng), và tối ưu UX (Toast notification, đổi mật khẩu trực tiếp). |

---

## Tài Liệu Tham Khảo

- [ASP.NET Core MVC — Microsoft Docs](https://learn.microsoft.com/aspnet/core/mvc)
- [MySQL 8.0 Reference Manual](https://dev.mysql.com/doc/refman/8.0/en/)
- [Bootstrap 5 Documentation](https://getbootstrap.com/docs/5.3/)

---

<p align="center">
  <b>The Drink VN</b> · Đồ án Website Bán Cà Phê & Nước Giải Khát<br>
  <i>C# ASP.NET MVC · SQL Server · Bootstrap</i>
</p>
