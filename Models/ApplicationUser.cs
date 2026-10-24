using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Green_Basket.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required(ErrorMessage = "Full name is required")]
        [StringLength(100)]
        [Display(Name = "Full Name")]
        public string FullName { get; set; } = string.Empty;
    }
}
