using Microsoft.EntityFrameworkCore.Query;
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
    [Table("tbl_Vendor")]
    public class Vendor : SqlTable
    {
        //-----------------------------------
        // SqlTable override
        //-----------------------------------
        [Key]
        [Column("Ven_Id")]
        public override int Id { get; set; }

        [Column("IsDeleted")]
        public override bool Deleted { get; set; }
        //------------ END overrides --------

        public string? Vendor_Code { get; set; }
        public string? Name { get; set; }
        public string? Address { get; set; }
        public string? Email { get; set; }
        public string? MobileNo { get; set; }
        public string? GstNo { get; set; }
        public string? Contact_Persons { get; set; }
        public string? Owner { get; set; }
        public string? Owner_Email { get; set; }
        public string? Owner_Mobile { get; set; }
        public string? Plant_Head { get; set; }
        public string? Plant_Email { get; set; }
        public string? Plant_Mobile { get; set; }
        public string? Quality_Manager { get; set; }
        public string? Quality_Email { get; set; }
        public string? Quality_Mobile { get; set; }
        public string? PDG_Manager { get; set; }
        public string? PDG_Email { get; set; }
        public string? PDG_Mobile { get; set; }
        public string? SCM_Manager { get; set; }
        public string? SCM_Email { get; set; }
        public string? SCM_Mobile { get; set; }
        public string? PRD_Manager { get; set; }
        public string? PRD_Email { get; set; }
        public string? PRD_Mobile { get; set; }
        public string? Service_Manager { get; set; }
        public string? Service_Email { get; set; }
        public string? Service_Mobile { get; set; }
        public string? Other_Cont_One { get; set; }
        public string? Other_Cont_OneEmail { get; set; }
        public string? Other_Cont_OneMobile { get; set; }
        public string? Other_Cont_Two { get; set; }
        public string? Other_Cont_TwoEmail { get; set; }
        public string? Other_Cont_TwoMobile { get; set; }
        public string? User_Name { get; set; }
        public string? Password { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
