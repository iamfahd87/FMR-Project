using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Family_Medical_Record.Models
{
    public class Doctor
    {
        [Key]
        public string Username { get; set; }

        [Required]
        [MaxLength(100)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(100)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        [Display(Name = "Date of Birth")]
        public DateTime DOB { get; set; }

        [Required]
        [MaxLength(10)]
        public string Gender { get; set; }

        [Required]
        [MaxLength(100)]
        public string Education { get; set; }

        [Required]
        [MaxLength(100)]
        public string Hospital { get; set; }

        [Required]
        [MaxLength(100)]
        public string Speciality { get; set; }

        public bool Confirmed { get; set; } = false;
    }
}