using System;
using System.ComponentModel.DataAnnotations;

namespace QMS.Core.Models
{
    public class RMTCDetailsViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Vendor")]
        public string? Vendor { get; set; }

        [Display(Name = "Product Category Ref")]
        public string? ProductCatRef { get; set; }

        [Display(Name = "Product Description")]
        public string? ProductDescription { get; set; }

        [Display(Name = "RMTC Date")]
        [DataType(DataType.Date)]
        public DateTime? RMTCDate { get; set; }

        [Display(Name = "Created Date")]
        public DateTime? CreatedDate { get; set; }

        [Display(Name = "Updated Date")]
        public DateTime? UpdatedDate { get; set; }

        [Display(Name = "Created By")]
        public string? CreatedBy { get; set; }

        [Display(Name = "Updated By")]
        public string? UpdatedBy { get; set; }

        [Display(Name = "Remarks")]
        public string? Remarks { get; set; }

        [Display(Name = "Housing Body (Metal / Plastic)")]
        public string? HousingBody { get; set; }

        [Display(Name = "Wires / Cables")]
        public string? WiresCables { get; set; }

        [Display(Name = "Diffuser / Lens")]
        public string? DiffuserLens { get; set; }

        [Display(Name = "PCB")]
        public string? PCB { get; set; }

        [Display(Name = "Connectors")]
        public string? Connectors { get; set; }

        [Display(Name = "Powder Coat")]
        public string? PowderCoat { get; set; }

        [Display(Name = "LED LM80 / Photo Biological")]
        public string? LEDLM80PhotoBiological { get; set; }

        [Display(Name = "LED Purchase Proof")]
        public string? LEDPurchaseProof { get; set; }

        [Display(Name = "Driver")]
        public string? Driver { get; set; }

        [Display(Name = "Pretreatment")]
        public string? Pretreatment { get; set; }

        [Display(Name = "Hardware (SS / MS)")]
        public string? Hardware { get; set; }

        [Display(Name = "Other Critical Items")]
        public string? OtherCriticalItems { get; set; }

        [Display(Name = "Attachment File Name")]
        public string? Filename { get; set; }
    }
}
