using System.ComponentModel.DataAnnotations.Schema;
namespace TechnoShop.Models {
    public class Product {
        public long? ProductID { get; set; }
        public string Name { get; set; } = String.Empty;
        public string Description { get; set; } = String.Empty;
        public string Brand { get; set; } = String.Empty;
        public string Specification { get; set; } = String.Empty;
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Price { get; set; }
        public string Category { get; set; } = String.Empty;
        public int StockQuantity { get; set; } = 0;
        public string ImageUrl { get; set; } = String.Empty;
    }
}