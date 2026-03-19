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
        
        public void CreateProduct(Product product)
        {
            context.Products.Add(product);
            context.SaveChanges();
        }
        
        public void UpdateProduct(Product product)
        {
            context.Products.Update(product);
            context.SaveChanges();
        }
        
        public void DeleteProduct(long productID)
        {
            var product = context.Products.Find(productID);
            if (product != null)
            {
                context.Products.Remove(product);
                context.SaveChanges();
            }
        }
        
        public Product? GetProduct(long productID)
        {
            return context.Products.Find(productID);
        }
    }
}
