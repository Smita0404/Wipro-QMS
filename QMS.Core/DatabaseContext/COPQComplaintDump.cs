using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using QMS.Core.DatabaseContext.Shared;

namespace QMS.Core.DatabaseContext
{
    [Table("tbl_COPQComplaintDump")]
    public class COPQComplaintDump : SqlTable
    {
        [Key]
        [Column("Id")]
        public override int Id { get; set; }

        [Column("IsDeleted")]
        public override bool Deleted { get; set; }

        [Column("CCCNDate")]
        public DateTime? CCCNDate { get; set; }

        [Column("ReportedBy")]
        public string? ReportedBy { get; set; }

        [Column("CLocation")]
        public string? CLocation { get; set; }

        [Column("CustName")]
        public string? CustName { get; set; }

        [Column("DealerName")]
        public string? DealerName { get; set; }

        [Column("CDescription")]
        public string? CDescription { get; set; }

        [Column("CStatus")]
        public string? CStatus { get; set; }

        [Column("Completion")]
        public string? Completion { get; set; }

        [Column("Remarks")]
        public string? Remarks { get; set; }

        [Column("CreatedDate")]
        public DateTime? CreatedDate { get; set; }

        [Column("UpdatedDate")]
        public DateTime? UpdatedDate { get; set; }

        [Column("CreatedBy")]
        public string? CreatedBy { get; set; }

        [Column("UpdatedBy")]
        public string? UpdatedBy { get; set; }
    }
}
