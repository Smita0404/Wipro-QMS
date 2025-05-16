using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QMS.Core.DatabaseContext;

namespace QMS.Core.Models
{
    public class ThirdPartyTestReportViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Product Code")]
        [StringLength(50)]
        public string? ProductCode { get; set; }

        [Display(Name = "Issue Date")]
        [DataType(DataType.Date)]
        public DateTime? IssueDate { get; set; }

        [Display(Name = "Expiry Date")]
        [DataType(DataType.Date)]
        public DateTime? ExpiryDate { get; set; }

        [Display(Name = "Report File Name")]
        public string? ReportFileName { get; set; }

        [Display(Name = "Remarks")]
        [StringLength(255)]
        public string? Remarks { get; set; }

        [Display(Name = "Vendor Code")]
        public string? VendorCode { get; set; }

        [Display(Name = "Vendor ID")]
        public int VendorID { get; set; }

        

        [Display(Name = "Created By")]
        public string? CreatedBy { get; set; }

        [Display(Name = "Created Date")]
        public DateTime? CreatedDate { get; set; }

        [Display(Name = "Updated By")]
        public string? UpdatedBy { get; set; }

        [Display(Name = "Updated Date")]
        public DateTime? UpdatedDate { get; set; }

        public bool IsDeleted { get; set; }
        [Display(Name = "Third Party Report ID")]
        public int ThirdPartyReportID { get; set; }

        // From Certificate Master
        public int CertificateID { get; set; }
        public string CertificateName { get; set; }


    }
}
