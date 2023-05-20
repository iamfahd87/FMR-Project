using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Family_Medical_Record.Models
{
    public class Record
    {
        [Key]
        public int RecordId { get; set; }

        [Required]
        public int PatientId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Title { get; set; }

        [Required]
        [MaxLength(100)]
        public string Type { get; set; }

        [Required]
        [MaxLength(100)]
        [Display(Name = "Hospital\\Lab")]
        public string Origin { get; set; }

        [Display(Name = "File Path")]
        public string FilePath { get; set; }

        public DateTime Date { get; set; }

        public virtual Patient Patient { get; set; }
    }
}