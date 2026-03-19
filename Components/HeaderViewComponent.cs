using Microsoft.AspNetCore.Mvc;
using TechnoShop.Models;

namespace TechnoShop.Components
{
  public class HeaderViewComponent : ViewComponent
  {
    private readonly IProductRepository repository;

    public HeaderViewComponent(IProductRepository repo)
    {
      repository = repo;
    }

    public IViewComponentResult Invoke()
    {
      var categories = repository.Products
        .Select(x => x.Category)
        .Distinct()
        .OrderBy(x => x)
        .ToList();

      return View(categories);
    }
  }
}
