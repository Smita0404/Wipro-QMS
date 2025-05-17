using QMS.Core.DatabaseContext.Shared;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QMS.Core.DatabaseContext
{
    [Table("tbl_DN_Tracker")]
    public class DNTracker : SqlTable
    {
        [Key]
        [Column("DNoteId")]
        public override int Id { get; set; }

        [Column("IsDeleted")]
        public override bool Deleted { get; set; }

        [Column("DNoteNumber")]
        [MaxLength(100)]
        public string? DNoteNumber { get; set; }

        [Column("DNoteCategory")]
        [MaxLength(100)]
        public string? DNoteCategory { get; set; }

        [Column("ProductCode")]
        [MaxLength(50)]
        public string? ProductCode { get; set; }

        [Column("ProductDescription")]
        [MaxLength(255)]
        public string? ProductDescription { get; set; }

        [Column("Wattage")]
        [MaxLength(50)]
        public string? Wattage { get; set; }

        [Column("DQty")]
        public int? DQty { get; set; }

        [Column("DRequisitionBy")]
        [MaxLength(100)]
        public string? DRequisitionBy { get; set; }

        [Column("Vendor")]
        [MaxLength(100)]
        public string? Vendor { get; set; }

        [Column("Remark")]
        public string? Remark { get; set; }

        [Column("CreatedBy")]
        [MaxLength(100)]
        public string? CreatedBy { get; set; }

        [Column("CreatedDate")]
        public DateTime? CreatedDate { get; set; }

        [Column("UpdatedBy")]
        [MaxLength(100)]
        public string? UpdatedBy { get; set; }

        [Column("UpdatedDate")]
        public DateTime? UpdatedDate { get; set; }
    }
}
