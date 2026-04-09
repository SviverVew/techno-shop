using Microsoft.AspNetCore.Http;
using TechnoShop.Models;
using TechnoShop.Models.VnPay;

namespace TechnoShop.Services
{
    public interface IVnpayService
    {
        string CreatePaymentUrl(PaymentInformationModel model, HttpContext context);
        PaymentResponseModel PaymentExecute(IQueryCollection collections);
    }
}