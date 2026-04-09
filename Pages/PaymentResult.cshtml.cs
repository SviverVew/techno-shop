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
private readonly IPaymentService _paymentService;
    private readonly IConfiguration _config;
    private readonly TechnoShopDbContext _context; // Thay bằng tên DbContext của bạn

    public string Message { get; set; } = "";
    public bool IsSuccess { get; set; } = false;

    public PaymentResultModel(IPaymentService paymentService, IConfiguration config, TechnoShopDbContext context)
    {
        _paymentService = paymentService;
            _config = config;
            _context = context;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var collections = Request.Query;
            string key1 = _config["ZaloPay:Key1"];

            // 1. Kiểm tra chữ ký bảo mật ZaloPay
            bool isValidSignature = _paymentService.ValidateSignature(collections, key1);

            if (isValidSignature)
            {
                string returnCode = collections["return_code"];
                string appTransId = collections["app_trans_id"];
                string transactionNo = collections["zptranstoken"];

                // 2. Sử dụng ExecutionStrategy để xử lý Deadlock (Retry logic)
                var strategy = _context.Database.CreateExecutionStrategy();

                await strategy.ExecuteAsync(async () =>
                {
                    // 3. Mở Transaction để đảm bảo tính toàn vẹn
                    using var transaction = await _context.Database.BeginTransactionAsync();
                    try
                    {
                        // Tìm đơn hàng dựa trên app_trans_id
                        var order = await _context.Orders
                            .TagWith("PaymentUpdateLock")
                            .FirstOrDefaultAsync(o => o.OrderGuid == appTransId);

                        if (order != null && order.PaymentStatus == "Pending")
                        {
                            if (returnCode == "1") // ZaloPay/VnPay thành công (return_code==1)
                            {
                                order.PaymentStatus = "Approved"; // Tự động duyệt khi thanh toán thành công
                                order.Shipped = false; // Chưa giao, admin sẽ confirm
                                order.VnpayTranNo = transactionNo; // dùng chung cột để lưu mã giao dịch
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
                        else if (order == null)
                        {
                            Message = "Không tìm thấy đơn hàng phù hợp để cập nhật trạng thái.";
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