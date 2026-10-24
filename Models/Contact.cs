using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Green_Basket.Models
{
    public class Contact
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Asset is required")]
        [Display(Name = "Linked Asset")]
        public int AssetId { get; set; }

        [ForeignKey("AssetId")]
        public virtual Asset? Asset { get; set; }

        [Required(ErrorMessage = "Contact name is required")]
        [StringLength(100)]
        [Display(Name = "Contact Name")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Contact role is required")]
        [StringLength(100)]
        [Display(Name = "Role / Title")]
        public string Role { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phone number is required")]
        [Phone]
        [StringLength(50)]
        [Display(Name = "Phone Number")]
        public string Phone { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress]
        [StringLength(150)]
        [Display(Name = "Email Address")]
        public string Email { get; set; } = string.Empty;
    }
}
