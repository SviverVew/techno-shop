using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechnoShop.Models;
using TechnoShop.Models.VnPay;
using TechnoShop.Services;

namespace TechnoShop.Controllers
{
    public class PaymentController : Controller
    {
        private readonly IVnpayService _vnPayService;
        private readonly TechnoShopDbContext _context;

        public PaymentController(IVnpayService vnPayService, TechnoShopDbContext context)
        {
            _vnPayService = vnPayService;
            _context = context;
        }

        [HttpPost]
        public IActionResult CreatePaymentUrlVnpay(PaymentInformationModel model)
        {
            var url = _vnPayService.CreatePaymentUrl(model, HttpContext);
            return Redirect(url);
        }

        [HttpGet]
        public async Task<IActionResult> PaymentCallbackVnpay()
        {
            var response = _vnPayService.PaymentExecute(Request.Query);
            
            // Cập nhật trạng thái order khi VNPAY callback
            // response.OrderId chứa vnp_TxnRef (OrderID)
            // response.VnPayResponseCode = "00" tức thành công
            if (!string.IsNullOrWhiteSpace(response.VnPayResponseCode) && 
                response.VnPayResponseCode == "00" &&
                !string.IsNullOrWhiteSpace(response.OrderId))
            {
                try
                {
                    var orderId = int.Parse(response.OrderId);
                    var order = await _context.Orders.FindAsync(orderId);
                    
                    if (order != null && order.PaymentStatus == "Pending")
                    {
                        order.PaymentStatus = "Approved"; // Tự động duyệt khi thanh toán VnPay thành công
                        order.VnpayTranNo = response.TransactionId ?? response.PaymentId;
                        await _context.SaveChangesAsync();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Lỗi cập nhật order: {ex.Message}");
                }
            }
            
            return View(response);
        }
    }
}