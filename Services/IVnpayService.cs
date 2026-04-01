using Microsoft.AspNetCore.Http;
using TechnoShop.Models;

namespace TechnoShop.Services
{
    public interface IVnpayService
    {
        string CreatePaymentUrl(HttpContext context, Order order);
        bool ValidateSignature(IQueryCollection collections, string hashSecret);
    }
}