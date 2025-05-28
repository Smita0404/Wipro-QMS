using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QMS.Core.Models
{
    public class PODetailViewModel
    {
        public int Id { get; set; }
        public string? Vendor { get; set; }
        public string? Material { get; set; }
        public string? ReferenceNo { get; set; }
        public string? PONo { get; set; }
        public DateTime? PODate { get; set; }
        public string? PRNo { get; set; }
        public string? BatchNo { get; set; }
        public string? POQty { get; set; }
        public string? BalanceQty { get; set; }
        public string? Destination { get; set; }
        public string? BalanceValue { get; set; }
        public string? CreatedBy { get; set; }

        public DateTime? CreatedDate { get; set; }

        public string? UpdatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public bool? IsDelete { get; set; }
    }
}
