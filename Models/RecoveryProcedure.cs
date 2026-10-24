using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Green_Basket.Models
{
    public class RecoveryProcedure
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Asset is required")]
        [Display(Name = "Linked Asset")]
        public int AssetId { get; set; }

        [ForeignKey("AssetId")]
        public virtual Asset? Asset { get; set; }

        [Required(ErrorMessage = "Procedure title is required")]
        [StringLength(200)]
        [Display(Name = "Procedure Title")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Recovery steps are required")]
        [Display(Name = "Step-by-Step Instructions")]
        public string Steps { get; set; } = string.Empty;
    }
}
