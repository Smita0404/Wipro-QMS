using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using QMS.Core.DatabaseContext.Shared;

namespace QMS.Core.DatabaseContext
{
    [Table("tbl_PDITracker")]
    public class PDITracker: SqlTable
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

        [StringLength(50)]
        public string? PC { get; set; }

        public DateTime? DispatchDate { get; set; }

        [StringLength(100)]
        public string? ProductCode { get; set; }

        [StringLength(550)]
        public string? ProductDescription { get; set; }

        [StringLength(100)]
        public string? BatchCodeVendor { get; set; }

        [StringLength(100)]
        public string? PONo { get; set; }

        public DateTime? PDIDate { get; set; }

        [StringLength(100)]
        public string? PDIRefNo { get; set; }

        public int? OfferedQty { get; set; }

        public int? ClearedQty { get; set; }

        public bool? BISCompliance { get; set; }

        [StringLength(100)]
        public string? InspectedBy { get; set; }

        [StringLength(255)]
        public string? Remark { get; set; }

        [StringLength(50)]
        public string? CreatedBy { get; set; }

        public DateTime? CreatedDate { get; set; }

        public string? UpdatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }
       
    }
}
