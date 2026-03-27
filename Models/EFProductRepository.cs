using System.Linq;

namespace TechnoShop.Models
{
    public class EFProductRepository : IProductRepository
    {
        private readonly TechnoShopDbContext context;

        public EFProductRepository(TechnoShopDbContext ctx)
        {
            context = ctx;
        }

        public IQueryable<Product> Products => context.Products;

        public void SaveProduct(Product product)
        {
            if (product.ProductID == 0)
            {
                context.Products.Add(product);
            }
            else
            {
                Product? dbEntry = context.Products
                    .FirstOrDefault(p => p.ProductID == product.ProductID);
                if (dbEntry != null)
                {
                    dbEntry.Name = product.Name;
                    dbEntry.Description = product.Description;
                    dbEntry.Price = product.Price;
                    dbEntry.Category = product.Category;
                    dbEntry.Brand = product.Brand;
                    dbEntry.StockQuantity = product.StockQuantity;
                    dbEntry.ImageUrl = product.ImageUrl;
                    dbEntry.Specification = product.Specification;
                }
            }
            context.SaveChanges();
        }

        public void DeleteProduct(long productID)
        {
            Product? dbEntry = context.Products.Find(productID);
            if (dbEntry != null)
            {
                context.Products.Remove(dbEntry);
                context.SaveChanges();
            }
        }

        public Product? GetProduct(long productID)
        {
            return context.Products.Find(productID);
        }
    }
}