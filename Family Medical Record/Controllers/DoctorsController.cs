using Family_Medical_Record.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Family_Medical_Record.Controllers
{
    [Authorize]
    public class DoctorsController : Controller
    {
        FMRDB db = new FMRDB();

        // GET: Doctors
        public ActionResult Index()
        {

            if(User.IsInRole("Admin"))
            {
                return View(db.Doctors.ToList());
            }

            string uname = User.Identity.GetUserName();

            var Doctors = db.Doctors.Where(x => x.Username != uname && x.Confirmed);

            return View(Doctors.ToList());
        }

        [ChildActionOnly]
        public ActionResult _DoctorsSearch(string search)
        {
            if (User.IsInRole("Admin"))
            {
                return PartialView(db.Doctors.Where(d => d.FirstName.Contains(search) || d.LastName.Contains(search)).ToList());
            }
            else if (User.IsInRole("Customer"))
            {
                return PartialView(db.Doctors.Where(d => (d.FirstName.Contains(search) || d.LastName.Contains(search) || d.Username.Contains(search)) && d.Confirmed == true).ToList());
            }

            return PartialView(null);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Approve(string username)
        {
            if(username == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var doctor = db.Doctors.Find(username);

            if(doctor == null)
            {
                return HttpNotFound();
            }

            if(doctor.Confirmed == true)
            {
                return RedirectToAction("Index");
            }

            doctor.Confirmed = true;

            db.Entry(doctor).State = System.Data.Entity.EntityState.Modified;

            db.SaveChanges();

            return RedirectToAction("Index");
        }

        // GET: Patients/UploadFile/5
        public ActionResult UploadFile()
        {
            if (!User.IsInRole("Doctor"))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Doctor doc = db.Doctors.Find(User.Identity.GetUserName());
            DoctorFile file = new DoctorFile();
            file.Doctor = doc;

            return View(file);
        }

        // POST: Patients/UploadFile/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UploadFile([Bind(Include = "Title")] DoctorFile doctorFile, HttpPostedFileBase FileUpload)
        {
            if(!User.IsInRole("Doctor"))
            {
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
            }

            doctorFile.Doctor = db.Doctors.Find(User.Identity.GetUserName());

            doctorFile.DoctorUsername = doctorFile.Doctor.Username;

            doctorFile.UploadedOn = DateTime.Now;

            if (FileUpload != null)
            {
                // Uploading File
                string file = Path.GetFileName(FileUpload.FileName);

                // Changing name of upload file
                file = doctorFile.Doctor.FirstName + "_" + doctorFile.Doctor.LastName + "_" + DateTime.Now.ToString("dd/MM/yyyy") + "_" + file;

                string MyPath = "/Files/";

                bool exists = Directory.Exists(Server.MapPath(MyPath));

                if (!exists)
                    Directory.CreateDirectory(Server.MapPath(MyPath));

                string path = Path.Combine(Server.MapPath(MyPath), file);

                FileUpload.SaveAs(path);

                doctorFile.FilePath = "/Files/" + file;

            }

            if (ModelState.IsValid)
            {
                db.DoctorFiles.Add(doctorFile);
                db.SaveChanges();
                return RedirectToAction("Index", "Manage");
            }
            return View(doctorFile);
        }

        // GET: Doctors/DownloadFile/5
        [HttpGet]
        public virtual ActionResult DownloadFile(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            DoctorFile doctorFile = db.DoctorFiles.Find(id);

            string uname = User.Identity.GetUserName();


            if (doctorFile == null || (doctorFile.DoctorUsername != uname && !User.IsInRole("Admin")))
            {
                return HttpNotFound();
            }

            string downloadName = doctorFile.Doctor.Username + "_" + doctorFile.UploadedOn + "." + doctorFile.FilePath.Split('.').Last();
            return File(doctorFile.FilePath, "application/octet-stream", downloadName);
        }

        public ActionResult DeleteFile(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            DoctorFile doctorFile = db.DoctorFiles.Find(id);

            string uname = User.Identity.GetUserName();


            if (doctorFile == null || (doctorFile.DoctorUsername != uname && !User.IsInRole("Admin")))
            {
                return HttpNotFound();
            }

            db.DoctorFiles.Remove(doctorFile);
            db.SaveChanges();

            return RedirectToAction("DoctorFiles", new {username = doctorFile.Doctor.Username});
        }

        // Return View For Files
        [Authorize]
        public ActionResult DoctorFiles(string username)
        {
            Doctor doctor = db.Doctors.Find(username);

            if (doctor == null || (doctor.Username != username && !User.IsInRole("Admin")))
            {
                return HttpNotFound();
            }

            return View(db.DoctorFiles.Where(x => doctor.Username == username).ToList());
        }
    }
}