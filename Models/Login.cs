using System.ComponentModel.DataAnnotations;

namespace TechnoShop.Models
{
    public class Login
    {
        [Required(ErrorMessage = "Vui lòng nhập tên đăng nhập")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        [UIHint("password")]
        public string? Password { get; set; }

        public string? ReturnUrl { get; set; } = "/";
    }
}
