using System.ComponentModel.DataAnnotations;

namespace TechnoShop.Models
{
    public class User
    {
        [Required]
        public string? Name { get; set; }

        [Required]
        [UIHint("email")]
        public string? Email { get; set; }

        [Required]
        [UIHint("password")]
        public string? Password { get; set; }

        [Required]
        [Compare(nameof(Password))]
        [UIHint("password")]
        public string? ConfirmPassword { get; set; }
    }
}
