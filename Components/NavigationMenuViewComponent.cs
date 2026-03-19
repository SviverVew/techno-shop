using Microsoft.AspNetCore.Mvc;
using TechnoShop.Models;
namespace TechnoShop.Components
{
  public class NavigationMenuViewComponent : ViewComponent
  {
    private readonly IProductRepository repository;
    public NavigationMenuViewComponent(IProductRepository repo)
    {
      repository = repo;
    }
    public IViewComponentResult Invoke()
    {
      ViewBag.SelectedCategory = RouteData?.Values["category"];
      return View(repository.Products
        .Select(x => x.Category)
        .Distinct()
        .OrderBy(x => x));
    }
  }
}