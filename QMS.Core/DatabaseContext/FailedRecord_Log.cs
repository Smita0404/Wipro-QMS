using QMS.Core.DatabaseContext.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QMS.Core.DatabaseContext
{
    [Table("tbl_FailedRecord_Log")]
    public class FailedRecord_Log : SqlTable
    {
        //-----------------------------------
        // SqlTable override
        //-----------------------------------
        [Key]
        [Column("Log_Id")]
        public override int Id { get; set; }

        [Column("IsDeleted")]
        public override bool Deleted { get; set; }
        //------------ END overrides --------
        public string? FileName { get; set; }
        public int? TotalRecords { get; set; }
        public int? ImportedRecords { get; set; }
        public int? FailedRecords { get; set; }
        public string? RecordType { get; set; }
        public string? UploadedBy { get; set; }
        public DateTime? UploadedAt { get; set; }
    }
}
