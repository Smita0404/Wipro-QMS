using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;

namespace QMS.Core.Models
{
    public class KaizenTrackerViewModel
    {
        public int Id { get; set; }

      
        public string? Vendor { get; set; }

       
        public string? KaizenTheme { get; set; }

       
        public string? KMonth { get; set; }

        public string? Team { get; set; }

     
        public string? KaizenFile { get; set; }

       

        public string? Remarks { get; set; }

        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }

        public bool? IsDeleted { get; set; }
    }
}
