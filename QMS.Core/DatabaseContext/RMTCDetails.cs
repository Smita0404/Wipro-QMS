using QMS.Core.DatabaseContext.Shared;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QMS.Core.DatabaseContext
{
    [Table("tbl_RMTCDetails")]
    public class RMTCDetails : SqlTable
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

        [Column("ProductCatRef")]
        public string? ProductCatRef { get; set; }

        [Column("ProductDescription")]
        public string? ProductDescription { get; set; }

        [Column("RMTCDate")]
        public DateTime? RMTCDate { get; set; }

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

        [Column("HousingBody")]
        public string? HousingBody { get; set; }

        [Column("WiresCables")]
        public string? WiresCables { get; set; }

        [Column("DiffuserLens")]
        public string? DiffuserLens { get; set; }

        [Column("PCB")]
        public string? PCB { get; set; }

        [Column("Connectors")]
        public string? Connectors { get; set; }

        [Column("PowderCoat")]
        public string? PowderCoat { get; set; }

        [Column("LEDLM80PhotoBiological")]
        public string? LEDLM80PhotoBiological { get; set; }

        [Column("LEDPurchaseProof")]
        public string? LEDPurchaseProof { get; set; }

        [Column("Driver")]
        public string? Driver { get; set; }

        [Column("Pretreatment")]
        public string? Pretreatment { get; set; }

        [Column("Hardware")]
        public string? Hardware { get; set; }

        [Column("OtherCriticalItems")]
        public string? OtherCriticalItems { get; set; }

        [Column("Filename")]
        public string? Filename { get; set; }
    }
}
