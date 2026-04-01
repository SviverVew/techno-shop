using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Linq;
using TechnoShop.Models;

namespace TechnoShop.Controllers
{
    [Authorize(Roles = "Admin")] // Chỉ Admin mới vào được link /Admin/...
    public class AdminController : Controller
    {
        private readonly IProductRepository repository;
        private readonly IOrderRepository orderRepository;
        private readonly IWebHostEnvironment env;

        public AdminController(IProductRepository repo, IOrderRepository orderRepo, IWebHostEnvironment env)
        {
            repository = repo;
            orderRepository = orderRepo;
            this.env = env;
        }

        // Trang danh sách sản phẩm: /Admin/Index
        public ViewResult Index() => View(repository.Products);

        // Trang quản lý đơn hàng: /Admin/Orders
        public ViewResult Orders() => View(orderRepository.Orders.OrderByDescending(o => o.OrderDate));

        [HttpPost]
        public IActionResult Approve(int orderId)
        {
            var order = orderRepository.GetOrder(orderId);
            if (order != null)
            {
                order.PaymentStatus = "Approved";
                orderRepository.SaveOrder(order);
                TempData["message"] = $"Đơn #{orderId} đã duyệt.";
            }
            return RedirectToAction("Orders");
        }

        [HttpPost]
        public IActionResult MarkShipped(int orderId)
        {
            var order = orderRepository.GetOrder(orderId);
            if (order != null)
            {
                order.Shipped = true;
                orderRepository.SaveOrder(order);
                TempData["message"] = $"Đơn #{orderId} đã xác nhận giao hàng.";
            }
            return RedirectToAction("Orders");
        }

        // Trang Sửa: /Admin/Edit?productId=5
        public ViewResult Edit(long productId) =>
            View(repository.Products.FirstOrDefault(p => p.ProductID == productId));

        [HttpPost]
        public IActionResult Edit(Product product, IFormFile? imageFile)
        {
            if (ModelState.IsValid)
            {
                if (imageFile != null && imageFile.Length > 0)
                {
                    var uploadsFolder = Path.Combine(env.WebRootPath, "images");
                    if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

                    var uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(imageFile.FileName)}";
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        imageFile.CopyTo(fileStream);
                    }
                    product.ImageUrl = $"/images/{uniqueFileName}";
                }

                repository.SaveProduct(product);
                TempData["message"] = $"{product.Name} đã được lưu thành công!";
                return RedirectToAction("Index");
            }
            return View(product);
        }

        // Trang Thêm mới: /Admin/Create
        public ViewResult Create() => View("Edit", new Product());

        [HttpPost]
        public IActionResult Delete(long productId)
        {
            repository.DeleteProduct(productId);
            TempData["message"] = "Đã xóa sản phẩm!";
            return RedirectToAction("Index");
        }
    }
}