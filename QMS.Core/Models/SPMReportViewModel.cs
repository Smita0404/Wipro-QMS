using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QMS.Core.Models
{
    public class SPMReportViewModel
    {
        public int Id { get; set; }

        public string? VendorDetail { get; set; }

        public string? FY { get; set; }

        public string? SPMQuarter { get; set; }

        public int? FinalStarRating { get; set; }

        public string? Top2Parameter { get; set; }

        public string? Lowest2Parameter { get; set; }

        public string? CreatedBy { get; set; }

        public string? UpdatedBy { get; set; }

        public DateTime? CreatedDate { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public string? Remarks { get; set; }

        public bool Deleted { get; set; }
    }
}

