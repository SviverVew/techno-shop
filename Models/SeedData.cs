using Microsoft.EntityFrameworkCore;
namespace TechnoShop.Models
{
  public static class SeedData
  {
    public static void EnsurePopulated(IApplicationBuilder app)
    {
      TechnoShopDbContext context = app.ApplicationServices
          .CreateScope().ServiceProvider.GetRequiredService<TechnoShopDbContext>();
      if (context.Database.GetPendingMigrations().Any())
      {
        context.Database.Migrate();
      }
      if (!context.Products.Any())
      {
        context.Products.AddRange(
            // Laptops
            new Product
            {
              Name = "Dell XPS 13",
              Description = "Laptop nhỏ gọn, mạnh mẽ với màn hình 13.3 inch FHD",
              Brand = "Dell",
              Category = "Laptop",
              Price = 24999000,
              Specification = "Intel i7, 16GB RAM, 512GB SSD",
              StockQuantity = 10,
              ImageUrl = "/images/dell-xps-13.jpg"
            },
            new Product
            {
              Name = "MacBook Pro 14",
              Description = "Laptop cao cấp với chip M1 Pro, hiệu năng vượt trội",
              Brand = "Apple",
              Category = "Laptop",
              Price = 47999000,
              Specification = "M1 Pro, 16GB RAM, 512GB SSD",
              StockQuantity = 5,
              ImageUrl = "/images/macbook-pro-14.jpg"
            },
            new Product
            {
              Name = "ASUS Vivobook 15",
              Description = "Laptop giá rẻ, thích hợp cho học sinh - sinh viên",
              Brand = "ASUS",
              Category = "Laptop",
              Price = 9999000,
              Specification = "AMD Ryzen 5, 8GB RAM, 256GB SSD",
              StockQuantity = 20,
              ImageUrl = "/images/asus-vivobook.jpg"
            },
            // Desktop Computers
            new Product
            {
              Name = "PC Gaming High-End",
              Description = "PC chơi game cao cấp, chiến mọi tựa game AAA",
              Brand = "Custom Build",
              Category = "Desktop",
              Price = 35000000,
              Specification = "Intel i9, RTX 3080 Ti, 32GB RAM, 1TB NVMe SSD",
              StockQuantity = 8,
              ImageUrl = "/images/pc-gaming-high-end.jpg"
            },
            new Product
            {
              Name = "PC Văn Phòng Mini",
              Description = "PC nhỏ gọn cho công việc văn phòng",
              Brand = "HP",
              Category = "Desktop",
              Price = 8999000,
              Specification = "Intel i5, Intel UHD Graphics, 8GB RAM, 256GB SSD",
              StockQuantity = 15,
              ImageUrl = "/images/pc-van-phong.jpg"
            },
            // Monitors
            new Product
            {
              Name = "LG UltraWide 34\"",
              Description = "Màn hình siêu rộng 34 inch cho lập trình viên",
              Brand = "LG",
              Category = "Monitor",
              Price = 12999000,
              Specification = "3440x1440, 100Hz, IPS Panel, USB-C",
              StockQuantity = 12,
              ImageUrl = "/images/lg-ultrawide.jpg"
            },
            new Product
            {
              Name = "Dell S2421H 24 inch",
              Description = "Màn hình 24 inch Full HD, tốt cho làm việc",
              Brand = "Dell",
              Category = "Monitor",
              Price = 4999000,
              Specification = "1920x1080, 60Hz, IPS, Flicker-Free",
              StockQuantity = 25,
              ImageUrl = "/images/dell-s2421h.jpg"
            },
            // Mice
            new Product
            {
              Name = "Logitech MX Master 3",
              Description = "Chuột không dây chuyên nghiệp, có thể ghép nhiều máy",
              Brand = "Logitech",
              Category = "Mouse",
              Price = 2499000,
              Specification = "Không dây, 8K DPI, Ghép nhiều máy",
              StockQuantity = 30,
              ImageUrl = "/images/logitech-mx-master.jpg"
            },
            new Product
            {
              Name = "Razer DeathAdder V3",
              Description = "Chuột gaming siêu nhẹ với độ chính xác cao",
              Brand = "Razer",
              Category = "Mouse",
              Price = 2999000,
              Specification = "Có dây, 30000 DPI, Trọng lượng 63g, RGB",
              StockQuantity = 18,
              ImageUrl = "/images/razer-deathadder.jpg"
            },
            new Product
            {
              Name = "AirPods Pro",
              Description = "Tai nghe không dây cao cấp với chống ồn chủ động",
              Brand = "Apple",
              Category = "Headphones",
              Price = 6999000,
              Specification = "ANC, Transparency Mode, H1 Chip",
              StockQuantity = 22,
              ImageUrl = "/images/airpods-pro.jpg"
            },
            // Keyboards
            new Product
            {
              Name = "Keychron K2",
              Description = "Bàn phím cơ không dây với pin bền",
              Brand = "Keychron",
              Category = "Keyboard",
              Price = 2199000,
              Specification = "Cơ, Gateron Brown, Không dây, Đèn LED",
              StockQuantity = 16,
              ImageUrl = "/images/keychron-k2.jpg"
            },
            new Product
            {
              Name = "Corsair K95 Platinum",
              Description = "Bàn phím cơ cao cấp cho gaming",
              Brand = "Corsair",
              Category = "Keyboard",
              Price = 5499000,
              Specification = "Cơ Cherry MX, Aluminum Frame, RGB",
              StockQuantity = 10,
              ImageUrl = "/images/corsair-k95.jpg"
            },
            // Storage
            new Product
            {
              Name = "Samsung 980 Pro 1TB",
              Description = "SSD NVMe siêu nhanh dành cho gaming và editing",
              Brand = "Samsung",
              Category = "Storage",
              Price = 3999000,
              Specification = "1TB, 7100MB/s Read, PCIe 4.0",
              StockQuantity = 20,
              ImageUrl = "/images/samsung-980-pro.jpg"
            },
            new Product
            {
              Name = "WD Blue 2TB HDD",
              Description = "Ổ cứng truyền thống, giá rẻ, dung lượng lớn",
              Brand = "Western Digital",
              Category = "Storage",
              Price = 1999000,
              Specification = "2TB, 256MB Cache, 7200 RPM",
              StockQuantity = 30,
              ImageUrl = "/images/wd-blue.jpg"
            }
        );
        context.SaveChanges();
      }
    }
  }
}