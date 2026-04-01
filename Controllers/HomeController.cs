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

    public ViewResult Index(string? category, string? searchQuery, int productPage = 1)
    {
      var products = repository.Products.AsQueryable();

      if (!string.IsNullOrEmpty(category))
      {
        products = products.Where(p => p.Category == category);
      }

      if (!string.IsNullOrEmpty(searchQuery))
      {
        var normalized = searchQuery.Trim().ToLower();
        products = products.Where(p =>
          p.Name.ToLower().Contains(normalized)
          || (p.Description != null && p.Description.ToLower().Contains(normalized))
          || (p.Brand != null && p.Brand.ToLower().Contains(normalized))
          || (p.Category != null && p.Category.ToLower().Contains(normalized))
        );
      }

      var totalItems = products.Count();

      var itemsOnPage = products
        .OrderBy(p => p.ProductID)
        .Skip((productPage - 1) * PageSize)
        .Take(PageSize);

      ViewData["SearchQuery"] = searchQuery;
      ViewData["CurrentCategory"] = category;

      return View(new ProductsListViewModel
      {
        Products = itemsOnPage,
        PagingInfo = new PagingInfo
        {
          CurrentPage = productPage,
          ItemsPerPage = PageSize,
          TotalItems = totalItems
        },
        CurrentCategory = category,
        SearchQuery = searchQuery
      });
    }

    public ViewResult Details(long productId)
    {
      var product = repository.Products.FirstOrDefault(p => p.ProductID == productId);
      if (product == null)
      {
        return View("NotFound");
      }
      return View(product);
    }

  }

}