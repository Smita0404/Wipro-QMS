using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using QMS.Core.DatabaseContext.Shared;
using QMS.Core.Models;

namespace QMS.Core.DatabaseContext
{
    [Table("tbl_COPQComplaintDump")]
    public class ComplaintDump_Service : SqlTable
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

    public class BulkCreateResult
    {
        public OperationResult Result { get; set; } = new();
        public List<(ComplaintViewModel Record, string Reason)> FailedRecords { get; set; } = new();
    }

    public class BulkCreatePOResult
    {
        public OperationResult Result { get; set; } = new();
        public List<(PendingPoViewModel Record, string Reason)> FailedRecords { get; set; } = new();
    }

    public class BulkCreateIndentResult
    {
        public OperationResult Result { get; set; } = new();
        public List<(IndentDumpViewModel Record, string Reason)> FailedRecords { get; set; } = new();
    }
}
