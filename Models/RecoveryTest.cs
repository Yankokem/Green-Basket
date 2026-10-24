using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Green_Basket.Models
{
    public class RecoveryTest
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Asset is required")]
        [Display(Name = "Target Asset")]
        public int AssetId { get; set; }

        [ForeignKey("AssetId")]
        public virtual Asset? Asset { get; set; }

        [Required(ErrorMessage = "Test date is required")]
        [DataType(DataType.Date)]
        [Display(Name = "Simulation / Test Date")]
        public DateTime TestDate { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "Result is required")]
        [StringLength(20)]
        [Display(Name = "Test Result")]
        public string Result { get; set; } = "Passed"; // "Passed" or "Failed"

        [Required(ErrorMessage = "Actual RTO is required")]
        [Range(0, 525600, ErrorMessage = "Actual RTO must be non-negative")]
        [Display(Name = "Actual Recovery Time (Minutes)")]
        public int ActualRtoMinutes { get; set; }

        [Required(ErrorMessage = "Actual RPO is required")]
        [Range(0, 525600, ErrorMessage = "Actual RPO must be non-negative")]
        [Display(Name = "Actual Data Loss (Minutes)")]
        public int ActualRpoMinutes { get; set; }

        [Display(Name = "Test Notes & Execution Observations")]
        public string? Notes { get; set; }

        [Display(Name = "Lessons Learned")]
        public string? LessonsLearned { get; set; }

        [NotMapped]
        public bool IsRtoMet => Asset == null || ActualRtoMinutes <= Asset.RtoMinutes;

        [NotMapped]
        public bool IsRpoMet => Asset == null || ActualRpoMinutes <= Asset.RpoMinutes;
    }
}
