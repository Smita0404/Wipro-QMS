using System;
using System.ComponentModel.DataAnnotations;

namespace QMS.Core.Models
{
    public class ImprovementTrackerViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Functional Area")]
        public string? FuncArea { get; set; }

        [Display(Name = "Existing Product / Issue")]
        public string? Issue { get; set; }

        [Display(Name = "ISO / EMS / OHSAS Problem")]
        public string? Problem { get; set; }

        [Display(Name = "Corrective Action")]
        public string? CorrectiveAction { get; set; }

        [Display(Name = "Responsible Person")]
        public string? Responsible { get; set; }

        [Display(Name = "Start / Implementation Date")]
        public DateTime? StartDate { get; set; }

        [Display(Name = "Improvements Achieved")]
        public string? ImprAchieved { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime? CreatedDate { get; set; }
        public string? CreatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
