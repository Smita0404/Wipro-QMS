using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using QMS.Core.DatabaseContext.Shared;

namespace QMS.Core.DatabaseContext
{
    [Table("tbl_CertificationDetail")]
    public class CertificationDetail : SqlTable
    {
        //-----------------------------------
        // SqlTable override
        //-----------------------------------
        [Key]
        [Column("VendorCertID")]
        public override int Id { get; set; }

        [Column("IsDeleted")]
        public override bool Deleted { get; set; }
        //------------ END overrides --------

        [Column("ProductCode")]
        [StringLength(50)]
        public string? ProductCode { get; set; }

        

        [Column("IssueDate")]
        public DateTime? IssueDate { get; set; }

        [Column("ExpiryDate")]
        public DateTime? ExpiryDate { get; set; }

        [Column("CertUpload")]
        public string? CertUpload { get; set; }

        [Column("UpdatedDate")]
        public DateTime? UpdatedDate { get; set; }

        [Column("CreatedDate")]
        public DateTime? CreatedDate { get; set; }

        [Column("CreatedBy")]
        [StringLength(50)]
        public string? CreatedBy { get; set; }

        [Column("UpdatedBy")]
        [StringLength(50)]
        public string? UpdatedBy { get; set; }

        [Column("Remarks")]
        [StringLength(255)]
        public string? Remarks { get; set; }
        [Column("VendorCode")]
        public string? VendorCode { get; set; }
        [Column("CertificateMasterId ")]
        public int CertificateMasterId { get; set; }
        public virtual CertificateMaster? CertificateMaster { get; set; }
        [Column("VendorID")]
        public int VendorID { get; set; }
        public virtual Vendor? Vendor { get; set; }
    }
}
