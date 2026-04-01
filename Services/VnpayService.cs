using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using TechnoShop.Infrastructure; 
using TechnoShop.Models;         

namespace TechnoShop.Services
{
    public class VnpayService : IVnpayService
    {
        private readonly IConfiguration _config;

        public VnpayService(IConfiguration config)
        {
            _config = config;
        }

        public string CreatePaymentUrl(HttpContext context, Order order) 
        {
            var vnpay = new VnPayLibrary();
            
            // Đảm bảo lấy đúng từ config
            string tmnCode = _config["Vnpay:TmnCode"];
            string hashSecret = _config["Vnpay:HashSecret"];
            string returnUrl = _config["Vnpay:ReturnUrl"];
            // Thay vì gọi config, ông viết thẳng link này vào:
            string baseUrl = "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html";

            vnpay.AddRequestData("vnp_Version", "2.1.0");
            vnpay.AddRequestData("vnp_Command", "pay");
            vnpay.AddRequestData("vnp_TmnCode", tmnCode);
            vnpay.AddRequestData("vnp_Amount", ((long)(order.TotalPrice * 100)).ToString()); // Nhân 100 và ép kiểu long
            vnpay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", "VND");
            vnpay.AddRequestData("vnp_IpAddr", "127.0.0.1"); // Fix cứng để test
            vnpay.AddRequestData("vnp_Locale", "vn");
            vnpay.AddRequestData("vnp_OrderInfo", "Thanh toan don hang " + order.OrderID);
            vnpay.AddRequestData("vnp_OrderType", "other");
            vnpay.AddRequestData("vnp_ReturnUrl", returnUrl);
            vnpay.AddRequestData("vnp_TxnRef", order.OrderID.ToString() + DateTime.Now.Minute.ToString()); // Dùng Ticks để không bao giờ trùng

            return vnpay.CreateRequestUrl(baseUrl, hashSecret);
        }

        public bool ValidateSignature(IQueryCollection collections, string hashSecret)
        {
            var vnpay = new VnPayLibrary();
            foreach (var key in collections.Keys)
            {
                var value = collections[key];
                if (!string.IsNullOrEmpty(key) && key.StartsWith("vnp_"))
                {
                    vnpay.AddResponseData(key, value);
                }
            }
            // Lấy hash từ VNPAY gửi về để kiểm tra tính toàn vẹn
            string vnp_SecureHash = collections["vnp_SecureHash"];
            return vnpay.ValidateSignature(vnp_SecureHash, hashSecret);
        }
    }
}