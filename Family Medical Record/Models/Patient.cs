using System;
using System.ComponentModel.DataAnnotations;

namespace Family_Medical_Record.Models
{
    public class Patient
    {
        [Key]
        public int PatientId { get; set; }

        public string Guardian { get; set; }

        [Required]
        [MaxLength(100)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(100)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        public string Picture { get; set; }

        [Required]
        [Display(Name = "Date of Birth")]
        public DateTime DOB { get; set; }

        [Required]
        [MaxLength(10)]
        public string Gender { get; set; }

        [MaxLength(5)]
        [Display(Name = "Blood Group")]
        public string BloodGroup { get; set; }

        [Display(Name = "Height (ft)")]
        public float Height { get; set; }

        [Display(Name = "Weight (Kg)")]
        public float Weight { get; set; }
    }
}