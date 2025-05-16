using QMS.Core.DatabaseContext.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QMS.Core.DatabaseContext
{
    [Table("tbl_UserRoles")]
    public class UserRoles : SqlTable
    {
        public static int Admin { get; set; }

        //-----------------------------------
        // SqlTable override
        //-----------------------------------
        [Key]
        [Column("Role_Id")]
        public override int Id { get; set; }

        [Column("IsDeleted")]
        public override bool Deleted { get; set; }
        //------------ END overrides --------

        public string? RoleName { get; set; }
    }
}
