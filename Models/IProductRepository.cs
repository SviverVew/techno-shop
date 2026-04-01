using System.Linq;

namespace TechnoShop.Models
{
    public interface IProductRepository
    {
        IQueryable<Product> Products { get; }
        void SaveProduct(Product product); // Kiểm tra kỹ chữ "Product"
        void DeleteProduct(long productID); // Dùng long cho đồng bộ
        Product? GetProduct(long productID);
    }
}