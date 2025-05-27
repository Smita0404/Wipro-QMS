using System;

namespace QMS.Core.Models
{
    public class COPQComplaintDumpViewModel
    {
        public int Id { get; set; }
        public bool Deleted { get; set; }

        public DateTime? CCCNDate { get; set; }
        public string? ReportedBy { get; set; }
        public string? CLocation { get; set; }
        public string? CustName { get; set; }
        public string? DealerName { get; set; }
        public string? CDescription { get; set; }
        public string? CStatus { get; set; }
        public string? Completion { get; set; }
        public string? Remarks { get; set; }

        public DateTime? CreatedDate { get; set; }
        public string? CreatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
