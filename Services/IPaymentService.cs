using Microsoft.AspNetCore.Http;
using TechnoShop.Models;

namespace TechnoShop.Services
{
    public class PaymentResult
    {
        public string CheckoutUrl { get; set; } = string.Empty;
        public string QrCodeUrl { get; set; } = string.Empty;
        public string AppTransId { get; set; } = string.Empty;
    }

    public interface IPaymentService
    {
        Task<PaymentResult> CreatePaymentAsync(HttpContext context, Order order);
        bool ValidateSignature(IQueryCollection collections, string secret);
    }
}
