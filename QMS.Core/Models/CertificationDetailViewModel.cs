using System;
using System.ComponentModel.DataAnnotations;

namespace QMS.Core.Models
{
    public class CertificationDetailViewModel
    {

        public int VendorCertID { get; set; }

        [Required(ErrorMessage = "Product Code is required.")]
        [Display(Name = "Product Code")]
        public string? ProductCode { get; set; }

        [Required(ErrorMessage = "Vendor ID is required.")]
        [Display(Name = "Vendor")]
        public int VendorID { get; set; }

        [Display(Name = "Issue Date")]
        [DataType(DataType.Date)]
        public DateTime? IssueDate { get; set; }

        [Display(Name = "Expiry Date")]
        [DataType(DataType.Date)]
        public DateTime? ExpiryDate { get; set; }

        [Display(Name = "Certificate File")]
        public string? CertUpload { get; set; }

        [Display(Name = "Remarks")]
        public string? Remarks { get; set; }

        [Display(Name = "Created By")]
        public string? CreatedBy { get; set; }

        [Display(Name = "Created Date")]
        public DateTime? CreatedDate { get; set; }

        [Display(Name = "Updated By")]
        public string? UpdatedBy { get; set; }

        [Display(Name = "Updated Date")]
        public DateTime? UpdatedDate { get; set; }
        [Display(Name = "certificateID")]
        public int CertificateID { get; set; }
        [Display(Name = "Certificate Name")]
        public string? CertificateName { get; set; }

        [Display(Name = "Vendor Code")]
        public string? VendorCode { get; set; }

    }
}
