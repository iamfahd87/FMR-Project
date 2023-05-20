using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Family_Medical_Record.Models;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;

namespace Family_Medical_Record.Controllers
{
    [Authorize]
    public class AppointmentsController : Controller
    {
        private FMRDB db = new FMRDB();

        // GET: Appointments
        public ActionResult Index()
        {
            string uname = User.Identity.GetUserName();


            if (User.IsInRole("Admin"))
            {
                return View(db.Appointments.ToList());
            }
            
            return View(db.Appointments.Where(a => a.Patient.Guardian == uname || a.Doctor == uname).ToList());
        }

        // GET: Appointments/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Appointment appointment = db.Appointments.Find(id);
            if (appointment == null)
            {
                return HttpNotFound();
            }


            string uname = User.Identity.GetUserName();
            if(User.IsInRole("Doctor"))
            {
                if(appointment.IsAccepted == false || (appointment.CancelledBy.Length > 1 && appointment.CancelledBy != uname))
                {
                    appointment.HasSeen = true;
                }
            }
            else if(User.IsInRole("Customer"))
            {
                if (appointment.IsAccepted == true || (appointment.CancelledBy.Length > 1 && appointment.CancelledBy != uname))
                {
                    appointment.HasSeen = true;
                }
            }

            db.Entry(appointment).State = EntityState.Modified;
            db.SaveChanges();

            return View(appointment);
        }

        // GET: Appointments/Create
        public ActionResult Create(string doctor)
        {
            string uname = User.Identity.GetUserName();

            ViewBag.PatientId = new SelectList((from p in db.Patients.Where(p => p.Guardian == uname)
                                                select new
                                                {
                                                    PatientId = p.PatientId,
                                                    FullName = p.FirstName + " " + p.LastName
                                                }), "PatientId", "FullName");

            if(doctor != null && doctor != "")
            {
                var selected = db.Doctors.Find(doctor);

                string SelectedDoc = selected.FirstName + " " + selected.LastName + " | " + selected.Hospital;

                ViewBag.Doctor = new SelectList((from d in db.Doctors
                                                 select new
                                                 {
                                                     DoctorUsername = d.Username,
                                                     FullName = d.FirstName + " " + d.LastName + " | " + d.Hospital
                                                 }), "DoctorUsername", "FullName", SelectedDoc);
            }
            else
            {
                ViewBag.Doctor = new SelectList((from d in db.Doctors
                                                 select new
                                                 {
                                                     DoctorUsername = d.Username,
                                                     FullName = d.FirstName + " " + d.LastName + " | " + d.Hospital
                                                 }), "DoctorUsername", "FullName");
            }
            return View();
        }

        // POST: Appointments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "AppointmentId,Doctor,PatientId,DateTime,HasSeen,IsAccepted,CancelledBy")] Appointment appointment)
        {
            if (ModelState.IsValid)
            {
                db.Appointments.Add(appointment);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            string uname = User.Identity.GetUserName();
            ViewBag.PatientId = new SelectList((from p in db.Patients.Where(p => p.Guardian == uname)
                                                select new
                                                { 
                                                    PatientId = p.PatientId,
                                                    FullName = p.FirstName + " " + p.LastName 
                                                }), "PatientId", "FullName", appointment.PatientId);

            ViewBag.Doctor = new SelectList((from d in db.Doctors
                                                     select new
                                                     {
                                                         DoctorUsername = d.Username,
                                                         FullName = d.FirstName + " " + d.LastName + " | " + d.Hospital
                                                     }), "DoctorUsername", "FullName", appointment.Doctor);

            return View(appointment);
        }

        // GET: Appointments/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Appointment appointment = db.Appointments.Find(id);
            if (appointment == null)
            {
                return HttpNotFound();
            }

            string uname = User.Identity.GetUserName();
            ViewBag.PatientId = new SelectList((from p in db.Patients.Where(p => p.Guardian == uname)
                                                select new
                                                {
                                                    PatientId = p.PatientId,
                                                    FullName = p.FirstName + " " + p.LastName
                                                }), "PatientId", "FullName", appointment.PatientId);

            ViewBag.Doctor = new SelectList((from d in db.Doctors
                                                     select new
                                                     {
                                                         DoctorUsername = d.Username,
                                                         FullName = d.FirstName + " " + d.LastName + " | " + d.Hospital
                                                     }), "DoctorUsername", "FullName", appointment.Doctor);
            return View(appointment);
        }

        // POST: Appointments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "AppointmentId,Doctor,PatientId,DateTime,HasSeen,IsAccepted,CancelledBy")] Appointment appointment)
        {
            if (ModelState.IsValid)
            {
                db.Entry(appointment).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            string uname = User.Identity.GetUserName();
            ViewBag.PatientId = new SelectList((from p in db.Patients.Where(p => p.Guardian == uname)
                                                select new
                                                {
                                                    PatientId = p.PatientId,
                                                    FullName = p.FirstName + " " + p.LastName
                                                }), "PatientId", "FullName", appointment.PatientId);

            ViewBag.Doctor = new SelectList((from d in db.Doctors
                                                     select new
                                                     {
                                                         DoctorUsername = d.Username,
                                                         FullName = d.FirstName + " " + d.LastName + " | " + d.Hospital
                                                     }), "DoctorUsername", "FullName", appointment.Doctor);
            return View(appointment);
        }

        // GET: Appointments/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Appointment appointment = db.Appointments.Find(id);
            if (appointment == null)
            {
                return HttpNotFound();
            }
            return View(appointment);
        }

        // POST: Appointments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Appointment appointment = db.Appointments.Find(id);
            db.Appointments.Remove(appointment);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Accept(int? id)
        {
            if(id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }


            var appointment = db.Appointments.Find(id);

            if(appointment == null)
            {
                return HttpNotFound();
            }

            if(appointment.CancelledBy.Length > 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }


            string uname = User.Identity.GetUserName();

            if (appointment.Doctor != uname)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            appointment.IsAccepted = true;

            appointment.HasSeen = false;

            db.Entry(appointment).State = EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        public ActionResult Cancel(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }


            var appointment = db.Appointments.Find(id);

            if (appointment == null)
            {
                return HttpNotFound();
            }

            if (appointment.IsAccepted)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }


            string uname = User.Identity.GetUserName();

            if ((User.IsInRole("Doctor") && appointment.Doctor != uname) || User.IsInRole("Customer") && appointment.Patient.Guardian != uname)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            appointment.CancelledBy = uname;

            appointment.HasSeen = false;

            db.Entry(appointment).State = EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        [ChildActionOnly]
        public ActionResult _AppointmentsPartial()
        {
            string uname = User.Identity.GetUserName();

            if(User.IsInRole("Customer"))
            {
                return PartialView(db.Appointments.Where(a => (a.IsAccepted == true || (a.CancelledBy != uname && a.CancelledBy.Length > 0)) && a.Patient.Guardian == uname && a.HasSeen == false).ToList());
            }

            return PartialView(db.Appointments.Where(a => ((a.IsAccepted == false && a.CancelledBy.Length < 1) || (a.CancelledBy != uname && a.CancelledBy.Length > 0)) && a.Doctor == uname && a.HasSeen == false).ToList());
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
