using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Green_Basket.Models
{
    public class RecoveryTask
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Asset is required")]
        [Display(Name = "Linked Asset")]
        public int AssetId { get; set; }

        [ForeignKey("AssetId")]
        public virtual Asset? Asset { get; set; }

        [Required(ErrorMessage = "Task title is required")]
        [StringLength(200)]
        [Display(Name = "Task Title")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Assignee name is required")]
        [StringLength(100)]
        [Display(Name = "Assigned To")]
        public string AssignedTo { get; set; } = string.Empty;

        [Required(ErrorMessage = "Status is required")]
        [StringLength(50)]
        [Display(Name = "Task Status")]
        public string Status { get; set; } = "Pending"; // "Pending", "In Progress", "Done"

        [DataType(DataType.Date)]
        [Display(Name = "Due Date")]
        public DateTime? DueDate { get; set; }
    }
}
