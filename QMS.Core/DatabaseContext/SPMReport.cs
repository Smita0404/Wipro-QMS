using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using QMS.Core.DatabaseContext.Shared;

namespace QMS.Core.DatabaseContext
{
    [Table("tbl_SPMReport")]
    public class SPMReport : SqlTable
    {
        [Key]
        public override int Id { get; set; }

        [Column("IsDeleted")]
        public override bool Deleted { get; set; }

        public string? VendorDetail { get; set; }

        public string? FY { get; set; }

        public string? SPMQuarter { get; set; }

        public int? FinalStarRating { get; set; }

        public string? Top2Parameter { get; set; }

        public string? Lowest2Parameter { get; set; }

        public DateTime? CreatedDate { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public string? CreatedBy { get; set; }

        public string? UpdatedBy { get; set; }

        public string? Remarks { get; set; }
    }
}
