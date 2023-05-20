using Family_Medical_Record.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Family_Medical_Record.Controllers
{
    [Authorize]
    public class PatientsController : Controller
    {
        private FMRDB db = new FMRDB();

        // GET: Patients
        public ActionResult Index()
        {
            if(!User.IsInRole("Admin"))
            {
                return RedirectToAction("MyFamily");
            }

            ViewBag.Title = "Patients";
            return View(db.Patients.ToList());
        }

        // GET: Patients/MyFamily
        public ActionResult MyFamily()
        {

            if(User.IsInRole("Admin"))
                return RedirectToAction("Index");

            string username = User.Identity.GetUserName();
            ViewBag.Title = username + "'s Family";
            return View("Index", db.Patients.Where(x => x.Guardian == username).ToList());
        }

        [ChildActionOnly]
        public ActionResult _PatientsSearch(string search)
        {
            if (User.IsInRole("Admin"))
            {
                return PartialView(db.Patients.Where(p => p.FirstName.Contains(search) || p.LastName.Contains(search) || p.Guardian.Contains(search)).ToList());
            }
            else if(User.IsInRole("Customer"))
            {
                string uname = User.Identity.GetUserName();
                return PartialView(db.Patients.Where(p => (p.FirstName.Contains(search) || p.LastName.Contains(search)) && p.Guardian == uname).ToList());
            }

            return PartialView(null);
        }

        // GET: Patients/DPView
        public ActionResult DPView(int? id)
        {

            if(id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            if (!User.IsInRole("Doctor"))
            {
                return HttpNotFound();
            }

            string uname = User.Identity.GetUserName();


            var patient = db.Patients.Where(p => p.PatientId == id);

            if(patient == null)
            {
                return HttpNotFound();
            }

            // Checking if patient has appointment for this doctor
            int count = db.Appointments.Where(a => a.PatientId == id && a.Doctor == uname).Count();

            if(count < 1)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ViewBag.Title = "Patient Details";
            return View("Index", patient.ToList());
        }

        // GET: Patients/Files/5
        public ActionResult Files(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Patient patient = db.Patients.Find(id);
            string uname = User.Identity.GetUserName();

            if(User.IsInRole("Doctor"))
            {
                // Checking if patient has appointment for this doctor
                int count = db.Appointments.Where(a => a.PatientId == id && a.Doctor == uname).Count();

                if (count < 1)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
            }
            else
            {
                if (patient == null || (patient.Guardian != uname && !User.IsInRole("Admin")))
                {
                    return HttpNotFound();
                }
            }

            return View(patient);
        }

        // GET: Patients/Create
        [Authorize(Roles = "Customer")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Patients/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Customer")]
        public ActionResult Create([Bind(Include = "FirstName,LastName,DOB,Gender,BloodGroup,Height,Weight")] Patient patient, HttpPostedFileBase imageUpload)
        {
            string username = User.Identity.GetUserName();

            patient.Guardian = username;

            if (imageUpload != null)
            {
                // Uploading Profile Picture
                string pic = System.IO.Path.GetFileName(imageUpload.FileName);
                
                // Adding patient name to pic name
                pic = patient.FirstName + "_" + patient.LastName + "_" + pic;

                string MyPath = "/images/";

                bool exists = System.IO.Directory.Exists(Server.MapPath(MyPath));

                if (!exists)
                    System.IO.Directory.CreateDirectory(Server.MapPath(MyPath));

                string path = System.IO.Path.Combine(Server.MapPath(MyPath), pic);

                imageUpload.SaveAs(path);

                patient.Picture = "/images/" + pic;

            }
            else
            {
                patient.Picture = "/Icons/User.png";
            }

            if (ModelState.IsValid)
            {
                db.Patients.Add(patient);
                db.SaveChanges();
                return RedirectToAction("MyFamily");
            }

            return View(patient);
        }

        // GET: Patients/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Patient patient = db.Patients.Find(id);
            string uname = User.Identity.GetUserName();

            if (patient == null || (patient.Guardian != uname && !User.IsInRole("Admin")))
            {
                return HttpNotFound();
            }
            return View(patient);
        }

        // POST: Patients/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PatientId,Picture,Guardian,FirstName,LastName,DOB,Gender,BloodGroup,Height,Weight")] Patient patient, HttpPostedFileBase imageUpload)
        {
            if(!User.IsInRole("Admin"))
            {
                string uname = User.Identity.GetUserName();
                patient.Guardian = uname;
            }

            if (imageUpload != null)
            {
                // Uploading Profile Picture
                string pic = Path.GetFileName(imageUpload.FileName);

                // Adding patient name to pic name
                pic = patient.FirstName + "_" + patient.LastName + "_" + pic;

                string MyPath = "/images/";

                bool exists = Directory.Exists(Server.MapPath(MyPath));

                if (!exists)
                    Directory.CreateDirectory(Server.MapPath(MyPath));

                string path = Path.Combine(Server.MapPath(MyPath), pic);

                imageUpload.SaveAs(path);

                patient.Picture = "/images/" + pic;
            }

            if (ModelState.IsValid)
            {
                db.Entry(patient).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("MyFamily");
            }
            return View(patient);
        }

        // GET: Patients/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Patient patient = db.Patients.Find(id);
            string uname = User.Identity.GetUserName();
            if (patient == null || (patient.Guardian != uname && !User.IsInRole("Admin")))
            {
                return HttpNotFound();
            }
            return View(patient);
        }

        // POST: Patients/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Patient patient = db.Patients.Find(id);
            string uname = User.Identity.GetUserName();
            
            if (patient.Guardian != uname && !User.IsInRole("Admin"))
            {
                return HttpNotFound();
            }

            db.Patients.Remove(patient);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: Patients/UploadFile/5
        public ActionResult UploadFile (int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Patient patient = db.Patients.Find(id);
            string uname = User.Identity.GetUserName();
            if (patient == null || (patient.Guardian != uname && !User.IsInRole("Admin")))
            {
                return HttpNotFound();
            }

            Record record = new Record();
            record.Patient = patient;

            return View (record);
        }

        // POST: Patients/UploadFile/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UploadFile([Bind(Include = "PatientId,Title, Type, Origin, Date")] Record record, HttpPostedFileBase FileUpload)
        {
            Patient patient = db.Patients.Find(record.PatientId);

            string uname = User.Identity.GetUserName();
            if (patient == null || (patient.Guardian != uname && !User.IsInRole("Admin")))
                return HttpNotFound();

            record.Patient = patient;

            if (FileUpload != null)
            {
                // Uploading File
                string file = Path.GetFileName(FileUpload.FileName);

                // Changing name of upload file
                file = patient.FirstName + "_" + patient.LastName + "_" + DateTime.Now.ToString("dd/MM/yyyy") + "_" + file;

                string MyPath = "/Files/";

                bool exists = Directory.Exists(Server.MapPath(MyPath));

                if (!exists)
                    Directory.CreateDirectory(Server.MapPath(MyPath));

                string path = Path.Combine(Server.MapPath(MyPath), file);

                FileUpload.SaveAs(path);

                record.FilePath = "/Files/" + file;

            }

            if (ModelState.IsValid)
            {
                db.Records.Add(record);
                db.SaveChanges();
                return RedirectToAction("Files", new { id = patient.PatientId });
            }
            return View(record);
        }

        // GET: Patients/DownloadFile/5
        [HttpGet]
        public virtual ActionResult DownloadFile(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Record record = db.Records.Find(id);

            string uname = User.Identity.GetUserName();

            if (User.IsInRole("Doctor"))
            {
                // Checking if patient has appointment for this doctor
                int count = db.Appointments.Where(a => a.PatientId == record.PatientId && a.Doctor == uname).Count();

                if (count < 1)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
            }
            else if (record == null || (record.Patient.Guardian != uname && !User.IsInRole("Admin")))
            {
                return HttpNotFound();
            }

            string downloadName = record.Type + "_" + record.Date + "." + record.FilePath.Split('.').Last();
            return File(record.FilePath, "application/octet-stream", downloadName);
        }

        public ActionResult DeleteFile(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Record record = db.Records.Find(id);

            string uname = User.Identity.GetUserName();


            if (record == null || (record.Patient.Guardian != uname && !User.IsInRole("Admin")))
            {
                return HttpNotFound();
            }

            db.Records.Remove(record);
            db.SaveChanges();

            return RedirectToAction("Files", new { id = record.PatientId });
        }

        // Return Partial View For Files
        [Authorize]
        [ChildActionOnly]
        public ActionResult _FilesPartial(int? id)
        {
            Patient patient = db.Patients.Find(id);
            string uname = User.Identity.GetUserName();

            if (User.IsInRole("Doctor"))
            {
                // Checking if patient has appointment for this doctor
                int count = db.Appointments.Where(a => a.PatientId == id && a.Doctor == uname).Count();

                if (count < 1)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
            }
            else
            {
                if (patient == null || (patient.Guardian != uname && !User.IsInRole("Admin")))
                {
                    return HttpNotFound();
                }
            }

            return PartialView(db.Records.Where(x => x.PatientId == id).ToList());
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
