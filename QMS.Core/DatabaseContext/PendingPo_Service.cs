using QMS.Core.DatabaseContext.Shared;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QMS.Core.DatabaseContext
{
    [Table("tbl_PODetails")]
    public class PendingPo_Service : SqlTable
    {
        // Overrides
        [Key]
        [Column("Id")]
        public override int Id { get; set; }

        [Column("IsDeleted")]
        public override bool Deleted { get; set; }

        // Table Columns
        public string? Vendor { get; set; }
        public string? Material { get; set; }

        [Column("ReferenceNo")]
        public string? ReferenceNo { get; set; }

        [Column("PONo")]
        public string? PONo { get; set; }

        [Column("PODate")]
        public DateTime? PODate { get; set; }

        public string? PRNo { get; set; }
        public string? BatchNo { get; set; }
        public string? POQty { get; set; }
        public string? BalanceQty { get; set; }
        public string? Destination { get; set; }
        public string? BalanceValue { get; set; }

        [Column("CreatedDate")]
        public DateTime? CreatedDate { get; set; }

        [Column("CreatedBy")]
        public string? CreatedBy { get; set; }

        [Column("UpdatedDate")]
        public DateTime? UpdatedDate { get; set; }

        [Column("UpdatedBy")]
        public string? UpdatedBy { get; set; }
    }
}
