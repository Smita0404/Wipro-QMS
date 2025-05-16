using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace QMS.Core.Models
{
    public class CSOTrackerViewModel
    {
        public int CSOId { get; set; }

        [DataType(DataType.Date)]
        public DateTime? CSOLogDate { get; set; }

        public string? CSONo { get; set; }

        public string? ClassAB { get; set; }

        public string? ProductCatRef { get; set; }

        public string? ProductDescription { get; set; }

        public string? SourceOfCSO { get; set; }

        public string? InternalExternal { get; set; }

        public string? PKDBatchCode { get; set; }

        public string? ProblemStatement { get; set; }

        public int? SuppliedQty { get; set; }

        public int? FailedQty { get; set; }

        public string? RootCause { get; set; }

        public string? CorrectiveAction { get; set; }

        public string? PreventiveAction { get; set; }

        [DataType(DataType.Date)]
        public DateTime? CSOsClosureDate { get; set; }

        public int? Aging { get; set; }

        public string? AttachmentCAPAReport { get; set; }

        // File upload support
        public IFormFile? AttachmentFile { get; set; }

        // Optional for soft delete (if needed for UI logic)
        public bool? IsDeleted { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
