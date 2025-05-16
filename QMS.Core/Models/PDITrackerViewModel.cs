using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QMS.Core.Models
{
    public class PDITrackerViewModel
    {
        public int Id { get; set; }

        public string? PC { get; set; }

        public DateOnly? DispatchDate { get; set; } // string to handle "dd/MM/yyyy" in frontend

        public string? ProductCode { get; set; }

        public string? ProductDescription { get; set; }

        public string? BatchCodeVendor { get; set; }

        public string? PONo { get; set; }

        public DateOnly? PDIDate { get; set; }

        public string? PDIRefNo { get; set; }

        public int? OfferedQty { get; set; }

        public int? ClearedQty { get; set; }

        public bool? BISCompliance { get; set; }

        public string? InspectedBy { get; set; }

        public string? Remark { get; set; }

        public string? CreatedBy { get; set; }

        public string? CreatedDate { get; set; }

        public string? UpdateBy { get; set; }

        public string? UpdatedDate { get; set; }

        public bool? IsDelete { get; set; }
    }

}
