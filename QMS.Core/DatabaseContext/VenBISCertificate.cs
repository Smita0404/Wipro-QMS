using Microsoft.EntityFrameworkCore.Query;
using QMS.Core.DatabaseContext.Shared;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace QMS.Core.DatabaseContext
{
    [Table("tbl_VenBISCertificate")]
    public class VenBISCertificate:SqlTable
    {
        //-----------------------------------
        // SqlTable override
        //-----------------------------------
        [Key]
        [Column("Id")]
        public override int Id { get; set; }

        [Column("IsDeleted")]
        public override bool Deleted { get; set; }
        //------------ END overrides --------

       
        [Display(Name = "Product Code")]
        public string? ProductCode { get; set; }

        
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

      
        [Display(Name = "Issue Date")]
        public DateTime? IssueDate { get; set; }

       
        [Display(Name = "Expiry Date")]
        public DateTime? ExpiryDate { get; set; }

        [Display(Name = "Certificate File")]
        public string? FileName { get; set; }

        public DateTime? CreatedDate { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public string? CreatedBy { get; set; }

        public string? UpdatedBy { get; set; }

        [Display(Name = "Certificate Detail")]
        public string? CertificateDetail { get; set; }
    }
}
