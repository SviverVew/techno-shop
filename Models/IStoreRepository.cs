namespace TechnoShop.Models {
    public interface IStoreRepository {
        IQueryable<Product> Products { get; }
    }
}