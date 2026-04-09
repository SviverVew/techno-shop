using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TechnoShop.Models;
using TechnoShop.Services;

namespace TechnoShop.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly IOrderRepository _repository;
        private readonly Cart _cart;
        private readonly IPaymentService _paymentService;
        private readonly TechnoShopDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public CheckoutController(
            IOrderRepository repository,
            Cart cart,
            IPaymentService paymentService,
            TechnoShopDbContext context,
            UserManager<IdentityUser> userManager)
        {
            _repository = repository;
            _cart = cart;
            _paymentService = paymentService;
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var order = new Order();
            ViewBag.HasSavedShipping = false;

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
                        order.Name = previousOrder.Name;
                        order.Email = previousOrder.Email;
                        order.Phone = previousOrder.Phone;
                        order.Line1 = previousOrder.Line1;
                        order.Line2 = previousOrder.Line2;
                        order.Line3 = previousOrder.Line3;
                        order.City = previousOrder.City;
                        order.State = previousOrder.State;
                        order.Zip = previousOrder.Zip;
                        order.Country = previousOrder.Country;
                        order.GiftWrap = previousOrder.GiftWrap;

                        ViewBag.HasSavedShipping = true;
                    }
                }
            }

            ViewBag.Cart = _cart;
            return View(order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(Order order)
        {
            ViewBag.Cart = _cart;
            ViewBag.HasSavedShipping = false;

            if (_cart == null || !_cart.Lines.Any())
            {
                ModelState.AddModelError(string.Empty, "Xin lỗi, giỏ hàng của bạn đang trống!");
            }

            ModelState.Remove("Lines");
            ModelState.Remove("order.Lines");

            if (ModelState.IsValid)
            {
                try
                {
                    order.Lines = _cart.Lines.ToArray();
                    order.TotalPrice = _cart.ComputeTotalValue();
                    order.OrderDate = DateTime.Now;
                    order.PaymentStatus = "Pending";

                    if (string.IsNullOrEmpty(order.Country))
                    {
                        order.Country = "Vietnam";
                    }

                    if (order.Zip == null)
                    {
                        order.Zip = string.Empty;
                    }

                    _repository.SaveOrder(order);

                    var paymentResult = await _paymentService.CreatePaymentAsync(HttpContext, order);

                    if (!string.IsNullOrWhiteSpace(paymentResult.AppTransId))
                    {
                        order.OrderGuid = paymentResult.AppTransId;
                        _repository.SaveOrder(order);
                    }

                    _cart.Clear();

                    var redirectUrl = $"/PaymentQr?qrUrl={Uri.EscapeDataString(paymentResult.QrCodeUrl)}&paymentUrl={Uri.EscapeDataString(paymentResult.CheckoutUrl)}";
                    return Redirect(redirectUrl);
                }
                catch (Exception ex)
                {
                    var innerMessage = ex.InnerException?.Message ?? ex.Message;
                    Console.WriteLine("--------------------------");
                    Console.WriteLine("LỖI SQL THẬT SỰ: " + innerMessage);
                    Console.WriteLine("--------------------------");

                    ModelState.AddModelError(string.Empty, "Lỗi lưu Database: " + innerMessage);
                }
            }

            return View(order);
        }
    }
}
