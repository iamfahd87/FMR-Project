using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Security.Policy;

namespace Family_Medical_Record.Models
{
    public class Appointment
    {
        [Key]
        public int AppointmentId { get; set; }

        [Required]
        public string Doctor { get; set; }

        [Required]
        [Display(Name = "Patient")]
        public int PatientId { get; set; }

        [Required]
        [Display(Name = "Appointment Date")]
        public DateTime DateTime { get; set; }

        [ScaffoldColumn(false)]
        public bool HasSeen { get; set; } = false;

        [ScaffoldColumn(false)]
        public bool IsAccepted { get; set; } = false;

        [ScaffoldColumn(false)]
        public string CancelledBy { get; set; } = "";

        public virtual Patient Patient { get; set; }
    }
}