using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Green_Basket.Models
{
    public class Asset
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Asset name is required")]
        [StringLength(150)]
        [Display(Name = "Asset Name")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Asset type is required")]
        [StringLength(100)]
        [Display(Name = "System Type")]
        public string Type { get; set; } = string.Empty;

        [Required(ErrorMessage = "Asset owner is required")]
        [StringLength(100)]
        [Display(Name = "System Owner / Lead")]
        public string Owner { get; set; } = string.Empty;

        [Required(ErrorMessage = "Priority level is required")]
        [StringLength(50)]
        [Display(Name = "Priority Level")]
        public string Priority { get; set; } = "Critical"; // Critical, High, Medium, Low

        [Required(ErrorMessage = "RTO (Recovery Time Objective) is required")]
        [Range(1, 525600, ErrorMessage = "RTO must be at least 1 minute")]
        [Display(Name = "RTO (Minutes)")]
        public int RtoMinutes { get; set; }

        [Required(ErrorMessage = "RPO (Recovery Point Objective) is required")]
        [Range(0, 525600, ErrorMessage = "RPO must be non-negative")]
        [Display(Name = "RPO (Minutes)")]
        public int RpoMinutes { get; set; }

        [Required(ErrorMessage = "Backup dependency details are required")]
        [StringLength(255)]
        [Display(Name = "Backup Dependency")]
        public string BackupDependency { get; set; } = string.Empty;

        // Navigation properties
        public virtual ICollection<RecoveryProcedure> RecoveryProcedures { get; set; } = new List<RecoveryProcedure>();
        public virtual ICollection<Contact> Contacts { get; set; } = new List<Contact>();
        public virtual ICollection<RecoveryTest> RecoveryTests { get; set; } = new List<RecoveryTest>();
        public virtual ICollection<RecoveryTask> RecoveryTasks { get; set; } = new List<RecoveryTask>();

        [NotMapped]
        public RecoveryTest? LatestTest => RecoveryTests
            .OrderByDescending(t => t.TestDate)
            .ThenByDescending(t => t.Id)
            .FirstOrDefault();

        [NotMapped]
        public int OpenTaskCount => RecoveryTasks.Count(t => !string.Equals(t.Status, "Done", StringComparison.OrdinalIgnoreCase));

        /// <summary>
        /// Derives overall disaster recovery readiness from the latest drill result and RTO/RPO targets.
        /// </summary>
        [NotMapped]
        public string ReadinessStatus
        {
            get
            {
                var latest = LatestTest;
                if (latest == null)
                {
                    return "Untested";
                }

                bool compliant = string.Equals(latest.Result, "Passed", StringComparison.OrdinalIgnoreCase)
                                 && latest.ActualRtoMinutes <= RtoMinutes
                                 && latest.ActualRpoMinutes <= RpoMinutes;

                return compliant ? "DR Ready" : "At Risk";
            }
        }
    }
}
