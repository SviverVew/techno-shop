using TechnoShop.Models;

namespace TechnoShop.Models {
    public class EFStoreRepository : IStoreRepository {
        private TechnoShopDbContext context;

        public EFStoreRepository(TechnoShopDbContext ctx) {
            context = ctx;
        }

        public IQueryable<Product> Products => context.Products;
    }
}