using QMS.Core.DatabaseContext.Shared;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QMS.Core.DatabaseContext
{
    [Table("tbl_ImprovementTracker")]
    public class ImprovementTracker : SqlTable
    {
        [Key]
        [Column("Id")]
        public override int Id { get; set; }

        [Column("IsDeleted")]
        public override bool Deleted { get; set; }

        

        [Column("FuncArea")]
        public string? FuncArea { get; set; }

        [Column("Issue")]
        public string? Issue { get; set; }

        [Column("Problem")]
        public string? Problem { get; set; }

        [Column("CorrectiveAction")]
        public string? CorrectiveAction { get; set; }

        [Column("Responsible")]
        public string? Responsible { get; set; }

        [Column("StartDate")]
        public DateTime? StartDate { get; set; }

        [Column("ImprAchieved")]
        public string? ImprAchieved { get; set; }

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
