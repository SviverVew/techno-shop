using Microsoft.AspNetCore.Mvc;

namespace TechnoShop.Components
{
  public class FooterViewComponent : ViewComponent
  {
    public IViewComponentResult Invoke()
    {
      return View();
    }
  }
}
