using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace QMS.Core.Models
{
    public class ThirdPartyInspectionViewModel
    {
        public int InspectionID { get; set; }

        [Display(Name = "Inspection Date")]
        public DateTime? InspectionDate { get; set; }

        [Display(Name = "Project Name")]
        [Required]
        public string ProjectName { get; set; }

        [Display(Name = "Inspector Name")]
        public string InspName { get; set; }

        public string ProductCode { get; set; }
        public string ProdDesc { get; set; }

        [Display(Name = "LOT Quantity")]
        public int? LOTQty { get; set; }

        [Display(Name = "Project Value")]
        public string ProjectValue { get; set; }

        public string Location { get; set; }
        public string Mode { get; set; }

        [Display(Name = "First Attempt")]
        public string FirstAttempt { get; set; }

        public string Remark { get; set; }

        [Display(Name = "Action Plan")]
        public string ActionPlan { get; set; }

        [Display(Name = "MOM Date")]
        public DateTime? MOMDate { get; set; }

        [Display(Name = "File Upload")]
        [Required(ErrorMessage = "Please upload an attachment.")]  // Make it required if needed
        public List<IFormFile>? AttachmentFiles { get; set; }

        public string? Attachment { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
    }
}