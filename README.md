# 🛒 TechnoShop

TechnoShop là một website bán hàng đơn giản được xây dựng bằng **ASP.NET Core MVC** và **Entity Framework Core**, hỗ trợ quản lý sản phẩm, giỏ hàng và đặt hàng.

---

## 🚀 Công nghệ sử dụng

* ASP.NET Core MVC (.NET 6)
* Entity Framework Core
* SQL Server
* Razor View Engine
* ASP.NET Identity (Authentication)

---

## 📦 Tính năng chính

* 🛍️ Hiển thị danh sách sản phẩm
* 🔎 Lọc sản phẩm theo danh mục (Category)
* 🛒 Giỏ hàng (Cart)
* 📦 Đặt hàng (Order)
* 🔐 Đăng nhập / đăng ký (Identity)
* 🌱 Seed dữ liệu mẫu khi chạy lần đầu

---

## ⚙️ Hướng dẫn cài đặt & chạy project

### 1. Clone source code

```bash
git clone https://github.com/SviverVew/techno-shop.git
cd techno-shop
```

---

### 2. Cài đặt dependencies

```bash
dotnet restore
```

---

### 3. Cấu hình Database

Mở file `appsettings.json` và chỉnh connection string:

```json
"ConnectionStrings": {
    "TechnoShopConnection": "Server=(local);Database=TechnoShopDB;Trusted_Connection=True;"
}
```

📌 Yêu cầu:

* Đã cài **SQL Server**
* Server chạy ở `localhost`

---

### 4. Tạo database (Migration)

```bash
dotnet ef database update
```

👉 Lệnh này sẽ:

* Tạo database `TechnoShopDB`
* Tạo toàn bộ bảng (Products, Orders, Identity, ...)

---

### 5. Chạy project

```bash
dotnet run
```

👉 Sau đó truy cập:

```
http://localhost:5000
```

---

## 🌱 Seed Data

Project sẽ tự động thêm dữ liệu mẫu (Products) khi chạy lần đầu.

👉 Không cần import dữ liệu thủ công

---

## 🧪 Cách test nhanh

1. Vào trang chủ
2. Chọn sản phẩm
3. Thêm vào giỏ hàng
4. Đi đến trang Cart
5. Thực hiện đặt hàng

---

## ⚠️ Lỗi thường gặp & cách xử lý

### ❌ Lỗi database

```bash
Cannot open database...
```

👉 Fix:

```bash
dotnet ef database drop
dotnet ef database update
```

---

### ❌ Lỗi migration

👉 Khi thay đổi model:

```bash
dotnet ef migrations add UpdateSomething
dotnet ef database update
```

---

### ❌ Lỗi thiếu view (Footer,...)

👉 Kiểm tra:

```
Views/Shared/Components/Footer/Default.cshtml
```

---

## 🔧 Ghi chú cho developer

* Khi thay đổi model → luôn tạo migration mới
* Không sửa trực tiếp DB bằng tay
* Seed data nằm trong: `SeedData.cs`


---
### Tóm lại
👉 Sau khi clone chỉ cần chạy:

```bash
dotnet restore
dotnet ef database update
dotnet run
```
