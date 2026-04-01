using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations.Schema; // <-- THÊM DÒNG NÀY ĐỂ DÙNG [NotMapped]

namespace TechnoShop.Models
{
    public class Order
    {
        [BindNever]
        public int OrderID { get; set; }

        public bool Shipped { get; set; } = false;
        [BindNever]
        [NotMapped] // <-- THÊM DÒNG NÀY VÀO ĐÂY ĐỂ EF KHÔNG ĐÒI TÌM BẢNG LƯU
        public ICollection<CartLine> Lines { get; set; } = new List<CartLine>();
        // -------------------

        [Required(ErrorMessage = "Vui lòng nhập tên")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập email")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        public string? Phone { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập địa chỉ")]
        public string? Line1 { get; set; }
        public string? Line2 { get; set; }
        public string? Line3 { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập thành phố")]
        public string? City { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tỉnh")]
        public string? State { get; set; }
        public string? Zip { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập quốc gia")]
        public string? Country { get; set; }

        public bool GiftWrap { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.Now;

        [BindNever]
        public decimal TotalPrice { get; set; }

        public string? PaymentStatus { get; set; } = "Pending"; 
        public string? PaymentMethod { get; set; } = "VNPAY";
        public string? VnpayTranNo { get; set; } 
        public string? OrderGuid { get; set; } = Guid.NewGuid().ToString();
    }
}