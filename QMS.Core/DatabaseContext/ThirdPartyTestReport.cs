using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using QMS.Core.DatabaseContext.Shared;

namespace QMS.Core.DatabaseContext
{
    [Table("tbl_VenThirdPartyTestReport")]
    public class ThirdPartyTestReport : SqlTable
    {
        [Key]
        [Column("ID")]
        public override int Id { get; set; }

        [Column("IsDeleted")]
        public override bool Deleted { get; set; }

        [Column("ProductCode")]
        [StringLength(50)]
        public string? ProductCode { get; set; }

        [Column("IssueDate")]
        public DateTime? IssueDate { get; set; }

        [Column("ExpiryDate")]
        public DateTime? ExpiryDate { get; set; }

        [Column("ReportFileName")]
        public string? ReportFileName { get; set; }

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

        [Column("VendorID")]
        public int VendorID { get; set; }
        [ForeignKey("VendorID")]
        public virtual Vendor? Vendor { get; set; }

        [Column("ThirdPartyReportID")]
        public int ThirdPartyReportID { get; set; }
        [ForeignKey("ThirdPartyReportID")]
        public virtual ThirdPartyTestReport? ThirdPartyTestReports { get; set; }
    }
}