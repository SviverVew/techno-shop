using Microsoft.AspNetCore.Mvc;

using TechnoShop.Models;

using TechnoShop.Models.ViewModels;

namespace TechnoShop.Controllers

{

  public class HomeController : Controller

  {

    private IProductRepository repository;

    public int PageSize = 8;

    public HomeController(IProductRepository repo)

    {

      repository = repo;

    }

    public ViewResult Index(string? category, int productPage = 1)
    => View(new ProductsListViewModel
    {
      Products = repository.Products
      .Where(p => category == null || p.Category == category)
      .OrderBy(p => p.ProductID)
      .Skip((productPage - 1) * PageSize)
      .Take(PageSize),
      PagingInfo = new PagingInfo
      {
        CurrentPage = productPage,
        ItemsPerPage = PageSize,
        TotalItems = category == null
          ? repository.Products.Count()
          : repository.Products.Where(e =>
              e.Category == category).Count()
      },
      CurrentCategory = category
    });

  }

}