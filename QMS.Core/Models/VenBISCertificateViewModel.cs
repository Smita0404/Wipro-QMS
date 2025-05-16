using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QMS.Core.Models
{
    public class VenBISCertificateViewModel
    {
        public int ID { get; set; }

        [Required]
        [Display(Name = "Product Code")]
        public string? ProductCode { get; set; }

        [Required]
        [Display(Name = "Vendor ID")]
        public int? VendorID { get; set; }

        [Display(Name = "Vendor Code")]
        public string? VendorCode { get; set; }

        [Display(Name = "BIS Section")]
        public string? BISSection { get; set; }

        [Display(Name = "R Number")]
        public string? RNumber { get; set; }

        [Display(Name = "Model Number")]
        public string? ModelNo { get; set; }

        [Display(Name = "Remarks")]
        public string? Remarks { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Issue Date")]
        public DateTime? IssueDate { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Expiry Date")]
        public DateTime? ExpiryDate { get; set; }

        [Display(Name = "Certificate File")]
        public string? FileName { get; set; }

        public DateTime? CreatedDate { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public string? CreatedBy { get; set; }

        public string? UpdatedBy { get; set; }

        public bool? IsDeleted { get; set; }

        [Display(Name = "Certificate Detail")]
        public string? CertificateDetail { get; set; }
    }

}
