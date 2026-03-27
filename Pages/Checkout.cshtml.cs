using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TechnoShop.Models;
using TechnoShop.Services;
using TechnoShop.Infrastructure;

namespace TechnoShop.Pages
{
    public class CheckoutModel : PageModel
    {
        private IOrderRepository repository;
        private IVnpayService _vnpayService;
        private TechnoShopDbContext _context;

        public CheckoutModel(IOrderRepository repoService, Cart cartService, 
                             IVnpayService vnpayService, TechnoShopDbContext context)
        {
            repository = repoService;
            UserCart = cartService; 
            _vnpayService = vnpayService;
            _context = context;
        }

        public Cart UserCart { get; set; }

        [BindProperty]
        public Order Order { get; set; } = new Order();

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            // 1. Dọn dẹp ModelState
            ModelState.Remove("UserCart");
            ModelState.Remove("Order.Lines");

            // 2. Kiểm tra giỏ hàng
            if (UserCart == null || UserCart.Lines.Count() == 0)
            {
                ModelState.AddModelError("", "Xin lỗi, giỏ hàng của bạn đang trống!");
                return Page();
            }

            // 3. Nếu dữ liệu Form hợp lệ
            if (ModelState.IsValid)
            {
                try
                {
                    // Gán dữ liệu
                    Order.Lines = UserCart.Lines.ToArray();
                    Order.TotalPrice = UserCart.ComputeTotalValue();
                    Order.OrderDate = DateTime.Now;
                    Order.PaymentStatus = "Pending";

                    // Lưu DB
                    repository.SaveOrder(Order);
                    await _context.SaveChangesAsync(); 

                    // Tạo URL thanh toán VNPAY
                    string vnpayUrl = _vnpayService.CreatePaymentUrl(HttpContext, Order);
                    
                    // Xóa giỏ hàng
                    UserCart.Clear();

                    // CHUYỂN HƯỚNG SANG VNPAY
                    return Redirect(vnpayUrl);
                }
                catch (Exception ex)
                {
                    // In lỗi chi tiết nhất có thể ra Terminal (Cửa sổ dotnet watch)
                    var innerException = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                    Console.WriteLine("--------------------------");
                    Console.WriteLine("LỖI SQL THẬT SỰ: " + innerException);
                    Console.WriteLine("--------------------------");
                    
                    ModelState.AddModelError("", "Lỗi lưu Database: " + innerException);
                    return Page();
                }
            }

            // 4. TRƯỜNG HỢP CUỐI: Nếu ModelState không hợp lệ hoặc trượt hết các if trên
            // Lệnh return này cực kỳ quan trọng để không bị lỗi CS0161
            return Page();
        }
    }
}