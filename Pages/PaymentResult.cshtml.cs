using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TechnoShop.Models;
using TechnoShop.Services;
using TechnoShop.Infrastructure;

namespace TechnoShop.Pages
{
    public class PaymentResultModel : PageModel
    {
        private readonly IVnpayService _vnpayService;
        private readonly IConfiguration _config;
        private readonly TechnoShopDbContext _context; // Thay bằng tên DbContext của bạn

        public string Message { get; set; } = "";
        public bool IsSuccess { get; set; } = false;

        public PaymentResultModel(IVnpayService vnpayService, IConfiguration config, TechnoShopDbContext context)
        {
            _vnpayService = vnpayService;
            _config = config;
            _context = context;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var collections = Request.Query;
            string hashSecret = _config["Vnpay:HashSecret"];
            
            // 1. Kiểm tra chữ ký bảo mật
            bool isValidSignature = _vnpayService.ValidateSignature(collections, hashSecret);

            if (isValidSignature)
            {
                string vnp_ResponseCode = collections["vnp_ResponseCode"];
                string vnp_OrderGuid = collections["vnp_TxnRef"];
                string vnp_TransactionNo = collections["vnp_TransactionNo"];

                // 2. Sử dụng ExecutionStrategy để xử lý Deadlock (Retry logic)
                var strategy = _context.Database.CreateExecutionStrategy();

                await strategy.ExecuteAsync(async () =>
                {
                    // 3. Mở Transaction để đảm bảo tính toàn vẹn
                    using var transaction = await _context.Database.BeginTransactionAsync();
                    try
                    {
                        // Tìm đơn hàng dựa trên GUID (Lock bản ghi này để tránh tranh chấp)
                        var order = await _context.Orders
                            .TagWith("PaymentUpdateLock") // Đánh dấu để dễ debug
                            .FirstOrDefaultAsync(o => o.OrderGuid == vnp_OrderGuid);

                        if (order != null && order.PaymentStatus == "Pending")
                        {
                            if (vnp_ResponseCode == "00") // Thanh toán thành công
                            {
                                order.PaymentStatus = "Success";
                                order.VnpayTranNo = vnp_TransactionNo;
                                Message = "Thanh toán thành công đơn hàng " + order.OrderID;
                                IsSuccess = true;
                            }
                            else
                            {
                                order.PaymentStatus = "Failed";
                                Message = "Thanh toán thất bại hoặc đã bị hủy.";
                            }

                            await _context.SaveChangesAsync();
                            await transaction.CommitAsync();
                        }
                    }
                    catch (Exception)
                    {
                        await transaction.RollbackAsync();
                        Message = "Lỗi hệ thống khi cập nhật trạng thái đơn hàng.";
                    }
                });
            }
            else
            {
                Message = "Chữ ký không hợp lệ, giao dịch có dấu hiệu giả mạo!";
            }

            return Page();
        }
    }
}