using QMS.Core.DatabaseContext.Shared;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QMS.Core.DatabaseContext
{
    [Table("tbl_KaizenTracker")]
    public class KaizenTracker : SqlTable
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

        [Column("Vendor")]
        public string? Vendor { get; set; }

        [Column("KaizenTheme")]
        public string? KaizenTheme { get; set; }

        [Column("KMonth")]
        public string? KMonth { get; set; }

        [Column("Team")]
        public string? Team { get; set; }

        [Column("KaizenFile")]
        public string? KaizenFile { get; set; }

        [Column("CreatedDate")]
        public DateTime? CreatedDate { get; set; }

        [Column("UpdatedDate")]
        public DateTime? UpdatedDate { get; set; }

        [Column("CreatedBy")]
        public string? CreatedBy { get; set; }

        [Column("UpdatedBy")]
        public string? UpdatedBy { get; set; }

        [Column("Remarks")]
        public string? Remarks { get; set; }
    }
}
