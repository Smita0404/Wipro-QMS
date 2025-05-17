using System;
using System.ComponentModel.DataAnnotations;

namespace QMS.Core.Models
{
    public class DNTrackerViewModel
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string? DNoteNumber { get; set; }

        [MaxLength(100)]
        public string? DNoteCategory { get; set; }

        [MaxLength(50)]
        public string? ProductCode { get; set; }

        [MaxLength(255)]
        public string? ProductDescription { get; set; }

        [MaxLength(50)]
        public string? Wattage { get; set; }

        public int? DQty { get; set; }

        [MaxLength(100)]
        public string? DRequisitionBy { get; set; }

        [MaxLength(100)]
        public string? Vendor { get; set; }

        public string? Remark { get; set; }

        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }

        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public bool IsDeleted { get; set; }
    }
}
