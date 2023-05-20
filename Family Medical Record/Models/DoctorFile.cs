using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Family_Medical_Record.Models
{
    public class DoctorFile
    {
        [Key]
        public int DoctorFileId { get; set; }

        [ScaffoldColumn(false)]
        [Display(Name = "Doctor")]
        public string DoctorUsername { get; set; }

        public string Title { get; set; }

        [ScaffoldColumn(false)]
        [Display(Name = "Uploaded On")]
        public DateTime UploadedOn { get; set; }

        [ScaffoldColumn(false)]
        [Display(Name = "File Path")]
        public string FilePath { get; set; }

        public virtual Doctor Doctor { get; set; }
    }
}