using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Family_Medical_Record.Models
{
    public class FMRDB : DbContext
    {
        public FMRDB() : base ("DefaultConnection") 
        {
        
        }

        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<DoctorFile> DoctorFiles { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Record> Records { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
    }
}