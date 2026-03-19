using Microsoft.AspNetCore.Mvc;
using TechnoShop.Models;
namespace TechnoShop.Controllers
{
  public class OrderController : Controller
  {
    private readonly IOrderRepository repository;
    private readonly Cart cart;
    public OrderController(IOrderRepository repoService, Cart cartService)
    {
      repository = repoService;
      cart = cartService;
    }
    public ViewResult Checkout() => View(new Order());
    [HttpPost]
    public IActionResult Checkout(Order order)
    {
      if (!cart.Lines.Any())
      {
        ModelState.AddModelError("", "Xin lỗi, giỏ hàng của bạn đang trống!");
      }
      if (ModelState.IsValid)
      {
        order.Lines = cart.Lines.ToArray();
        repository.SaveOrder(order);
        cart.Clear();
        return RedirectToPage("/Completed", new { orderId = order.OrderID });
      }
      else
      {
        return View();
      }
    }
  }
}
