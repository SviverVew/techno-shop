using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
namespace TechnoShop.Models
{
  public class TechnoShopDbContext : IdentityDbContext
  {
    public TechnoShopDbContext(DbContextOptions<TechnoShopDbContext> options)
        : base(options) { }
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Order> Orders => Set<Order>();
  }
}