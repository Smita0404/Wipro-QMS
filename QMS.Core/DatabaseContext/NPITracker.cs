using QMS.Core.DatabaseContext.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QMS.Core.DatabaseContext
{
    [Table("tbl_NPI_Tracker")]
    public class NPITracker : SqlTable
    {
        //-----------------------------------
        // SqlTable override
        //-----------------------------------
        [Key]
        [Column("NPI_TracID")]
        public override int Id { get; set; }

        [Column("IsDeleted")]
        public override bool Deleted { get; set; }
        //------------ END overrides --------
        public string? PC { get; set; }
        public string? Vendor { get; set; }
        public string? Prod_Category { get; set; }
        public string? Product_Code { get; set; }
        public string? Product_Des { get; set; }
        public string? Wattage { get; set; }
        public string? NPI_Category { get; set; }
        public DateTime? Offered_Date { get; set; }
        public DateTime? Released_Date { get; set; }
        public string? Releasded_Day { get; set; }
        public string? Validation_Rep_No { get; set; }
        public string? Customer_Comp { get; set; }
        public string? Remark { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
