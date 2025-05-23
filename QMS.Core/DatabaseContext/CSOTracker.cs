using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using QMS.Core.DatabaseContext.Shared;

namespace QMS.Core.DatabaseContext
{
    

    [Table("tbl_CSOTracker")]
    public class CSOTracker: SqlTable
    {
        //-----------------------------------
        // SqlTable override
        //-----------------------------------
        [Key]
        [Column("CSOId")]
        public override int Id { get; set; }

        [Column("IsDeleted")]
        public override bool Deleted { get; set; }
        //------------ END overrides --------

       public DateTime? CSOLogDate { get; set; }

        [StringLength(50)]
        public string? CSONo { get; set; }

        [StringLength(5)]
        public string? ClassAB { get; set; }

        [StringLength(100)]
        public string? ProductCatRef { get; set; }

        [StringLength(255)]
        public string? ProductDescription { get; set; }

        [StringLength(100)]
        public string? SourceOfCSO { get; set; }

        [StringLength(50)]
        public string? InternalExternal { get; set; }

        [StringLength(100)]
        public string? PKDBatchCode { get; set; }

        [StringLength(500)]
        public string? ProblemStatement { get; set; }

        public int? SuppliedQty { get; set; }

        public int? FailedQty { get; set; }

        [StringLength(500)]
        public string? RootCause { get; set; }

        [StringLength(500)]
        public string? CorrectiveAction { get; set; }

        [StringLength(500)]
        public string? PreventiveAction { get; set; }

        public DateTime? CSOsClosureDate { get; set; }

        public int? Aging { get; set; }

        [StringLength(255)]
        public string? AttachmentCAPAReport { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? UpdatedBy { get; set; }
    }

}
