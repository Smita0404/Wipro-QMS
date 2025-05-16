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
    [Table("tbl_Users")]
    public class Users : SqlTable
    {
        //-----------------------------------
        // SqlTable override
        //-----------------------------------
        [Key]
        [Column("User_Id")]
        public override int Id { get; set; }

        [Column("IsDeleted")]
        public override bool Deleted { get; set; }
        //------------ END overrides --------

        [StringLength(250)]
        public string? Name { get; set; }

        public string? Email { get; set; }

        public string? Username { get; set; }

        public string? Password { get; set; }

        public int? RoleId { get; set; }

        public string? AdId { get; set; }

        public string? Designation { get; set; }

        public int? DivisionId { get; set; }

        public string? MobileNo { get; set; }

        public string? User_Type { get; set; }

        [ForeignKey("RoleId")]
        public virtual UserRoles? UserRoles { get; set; }

        //[ForeignKey("DivisionId")]
        //public virtual Division? Divisions { get; set; }
    }
}
