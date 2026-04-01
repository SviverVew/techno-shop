using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TechnoShop.Models;
using TechnoShop.Services;
using TechnoShop.Infrastructure;

namespace TechnoShop.Pages
{
    public class CheckoutModel : PageModel
    {
        private IOrderRepository repository;
    private IPaymentService _paymentService;
    private TechnoShopDbContext _context;
    private UserManager<IdentityUser> _userManager;

    public CheckoutModel(IOrderRepository repoService, Cart cartService, 
                         IPaymentService paymentService, TechnoShopDbContext context,
                         UserManager<IdentityUser> userManager)
    {
        repository = repoService;
        UserCart = cartService; 
        _paymentService = paymentService;
        _context = context;
        _userManager = userManager;
    }

        public Cart UserCart { get; set; }

        [BindProperty]
        public Order Order { get; set; } = new Order();

        public bool HasSavedShipping { get; set; }

        public async Task OnGetAsync()
        {
            HasSavedShipping = false;

            if (User.Identity?.IsAuthenticated == true)
            {
                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser != null && !string.IsNullOrEmpty(currentUser.Email))
                {
                    var previousOrder = await _context.Orders
                        .Where(o => o.Email == currentUser.Email)
                        .OrderByDescending(o => o.OrderDate)
                        .FirstOrDefaultAsync();

                    if (previousOrder != null)
                    {
                        Order.Name = previousOrder.Name;
                        Order.Email = previousOrder.Email;
                        Order.Phone = previousOrder.Phone;
                        Order.Line1 = previousOrder.Line1;
                        Order.Line2 = previousOrder.Line2;
                        Order.Line3 = previousOrder.Line3;
                        Order.City = previousOrder.City;
                        Order.State = previousOrder.State;
                        Order.Zip = previousOrder.Zip;
                        Order.Country = previousOrder.Country;
                        Order.GiftWrap = previousOrder.GiftWrap;

                        HasSavedShipping = true;
                    }
                }
            }
        }

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

                    // Nếu bỏ trường Country trong form (trong nước), gán mặc định
                    if (string.IsNullOrEmpty(Order.Country))
                    {
                        Order.Country = "Vietnam";
                    }

                    // Ngăn NULL ở Zip nếu cột không cho phép hoặc không cần thiết
                    if (Order.Zip == null)
                    {
                        Order.Zip = string.Empty;
                    }

                    // Lưu DB (chưa có app_trans_id)
                    repository.SaveOrder(Order);

                    // Tạo URL thanh toán ZaloPay
                    var paymentResult = await _paymentService.CreatePaymentAsync(HttpContext, Order);

                    // Đảm bảo app_trans_id gán cho Order.OrderGuid (dùng để lookup callback)
                    if (!string.IsNullOrWhiteSpace(paymentResult.AppTransId))
                    {
                        Order.OrderGuid = paymentResult.AppTransId;
                        repository.SaveOrder(Order); // cập nhật OrderGuid
                    }

                    // Xóa giỏ hàng
                    UserCart.Clear();

                    // Nếu có QR code URL, chuyển sang trang trung gian hiển thị QR
                    return RedirectToPage("/PaymentQr", new { qrUrl = paymentResult.QrCodeUrl, paymentUrl = paymentResult.CheckoutUrl });
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