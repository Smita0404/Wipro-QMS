using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QMS.Core.Models
{
    public class BisProjectTracViewModel
    {
        public int Id { get; set; }
        public bool Deleted { get; set; }
        public string? Financial_Year { get; set; }
        public string? Mon_Pc { get; set; }
        public string? Nat_Project { get; set; }
        public string? Lea_Model_No { get; set; }
        public string? No_Seri_Add { get; set; }
        public string? Cat_Ref_Lea_Model { get; set; }
        public string? Section { get; set; }
        public string? Manuf_Location { get; set; }
        public string? CCL_Id { get; set; }
        public string? Lab { get; set; }
        public string? Report_Owner { get; set; }
        public DateTime? Start_Date { get; set; }
        public DateTime? Comp_Date { get; set; }
        public string? Test_Duration { get; set; }
        public DateTime? Submitted_Date { get; set; }
        public DateTime? Received_Date { get; set; }
        public string? Bis_Duration { get; set; }
        public DateTime? Dispatch_Date { get; set; }
        public string? Remark { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
