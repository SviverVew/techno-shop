using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechnoShop.Models;
using System.Linq;

namespace TechnoShop.Controllers
{
    [Authorize(Roles = "Admin")] // Chỉ Admin mới vào được link /Admin/...
    public class AdminController : Controller
    {
        private IProductRepository repository;

        public AdminController(IProductRepository repo) => repository = repo;

        // Trang danh sách sản phẩm: /Admin/Index
        public ViewResult Index() => View(repository.Products);

        // Trang Sửa: /Admin/Edit?productId=5
        public ViewResult Edit(long productId) =>
            View(repository.Products.FirstOrDefault(p => p.ProductID == productId));

        [HttpPost]
        public IActionResult Edit(Product product)
        {
            if (ModelState.IsValid) {
                repository.SaveProduct(product);
                TempData["message"] = $"{product.Name} đã được lưu thành công!";
                return RedirectToAction("Index");
            } else {
                // Nếu dữ liệu nhập vào lỗi (ví dụ bỏ trống tên), trả lại View để sửa
                return View(product);
            }
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