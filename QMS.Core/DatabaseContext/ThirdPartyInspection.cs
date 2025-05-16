using QMS.Core.DatabaseContext.Shared;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QMS.Core.DatabaseContext
{
    [Table("tbl_ThirdPartyInspection")]
    public class ThirdPartyInspection : SqlTable
    {
        //-----------------------------------
        // SqlTable override
        //-----------------------------------
        [Key]
        [Column("InspectionID")]
        public override int Id { get; set; }

        [Column("IsDeleted")]
        public override bool Deleted { get; set; }
        //------------ END overrides --------

        public DateTime? InspectionDate { get; set; }
        public string ProjectName { get; set; }
        public string InspName { get; set; }
        public string ProductCode { get; set; }
        public string ProdDesc { get; set; }
        public int? LOTQty { get; set; }
        public string ProjectValue { get; set; }
        public string Location { get; set; }
        public string Mode { get; set; }
        public string FirstAttempt { get; set; }
        public string Remark { get; set; }
        public string ActionPlan { get; set; }
        public DateTime? MOMDate { get; set; }
        public string? Attachment { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
