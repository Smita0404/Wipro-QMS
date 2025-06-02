using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QMS.Core.Models
{
    public class IndentDumpViewModel
    {
        public int Id { get; set; }
        public bool Deleted { get; set; }
        public string? Indent_No { get; set; }
        public DateTime? Indent_Date { get; set; }
        public string? Business_Unit { get; set; }
        public string? Vertical { get; set; }
        public string? Branch { get; set; }
        public string? Indent_Status { get; set; }
        public string? End_Cust_Name { get; set; }
        public string? Complaint_Id { get; set; }
        public string? Customer_Code { get; set; }
        public string? Customer_Name { get; set; }
        public DateTime? Bill_Req_Date { get; set; }
        public string? Created_By { get; set; }
        public string? Wipro_Commit_Date { get; set; }
        public string? Material_No { get; set; }
        public string? Item_Description { get; set; }
        public int? Quantity { get; set; }
        public string? Price { get; set; }
        public string? Final_Price { get; set; }
        public string? SapSoNo { get; set; }
        public int? CreateSoQty { get; set; }
        public int? Inv_Qty { get; set; }
        public string? Inv_Value { get; set; }
        public string? WiproCatelog_No { get; set; }
        public string? Batch_Code { get; set; }
        public DateTime? Batch_Date { get; set; }
        public string? Main_Prodcode { get; set; }
        public string? User_Name { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
