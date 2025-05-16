using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QMS.Core.DatabaseContext;

namespace QMS.Core.Models
{
   public class CertificateMasterViewModel
    {
        public int CertificateID { get; set; }
        public string? CertificateName { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public bool? IsDeleted { get; set; }
    
    }

}
