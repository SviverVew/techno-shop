namespace TechnoShop.Models
{
    public interface IProductRepository
    {
        IQueryable<Product> Products { get; }
        void CreateProduct(Product product);
        void UpdateProduct(Product product);
        void DeleteProduct(long productID);
        Product? GetProduct(long productID);
    }
}
