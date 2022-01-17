using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BusinesssTrinitySP01.Models;

namespace BusinesssTrinitySP01.Controllers
{
    public class JobApplicationsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: JobApplications
        public ActionResult Index()
        {
            var jobApplications = db.JobApplications.Include(j => j.Applicant).Include(j => j.Job);
            return View(jobApplications.ToList());
        }

        public ActionResult CheckApplicant(Applicant myapp)
        {
            var applicant = db.applicants.Where(x => x.RSAID == myapp.RSAID).FirstOrDefault();
            if (applicant != null)
            {
                var jobApplications = db.JobApplications.Where(x => x.AppID == applicant.AppID).Include(j => j.Applicant).Include(j => j.Job);
                return RedirectToAction("MyApplications", jobApplications);
            }

            ViewBag.NoApp = "No Applicant was found of this ID";
            return View();
        }

        public ActionResult ViewApplications()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ViewApplications(Applicant appl)
        {
            var app = db.applicants.Where(x => x.RSAID == appl.RSAID).FirstOrDefault();
            if(app == null)
            {
                ViewBag.NotFound = "The Applicant ID was not found. Please double check or create a new profile.";
                return View();
            }
            else
            {
                return RedirectToAction("MyApplications", new { id = app.AppID });
            }
        }

        public ActionResult MyApplications(int id)
        {
            
            var jobApplications = db.JobApplications.Where(x => x.AppID == id).ToList();
            return View(jobApplications);
        }

        // GET: JobApplications/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            JobApplications jobApplications = db.JobApplications.Find(id);
            if (jobApplications == null)
            {
                return HttpNotFound();
            }
            return View(jobApplications);
        }

        // GET: JobApplications/Create
        public ActionResult Create()
        {
          
            var jobs = db.jobs.Where(x => x.ClosingDate >= DateTime.Now).ToList();
            JobApplications jobApplications = new JobApplications();
            jobApplications.jobs = jobs;
            return View(jobApplications);
        }

        // POST: JobApplications/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "JobAppID,JobID,AppID,AppliedDate,Status,Resume")] JobApplications jobApplications)
        {
            if (ModelState.IsValid)
            {
                db.JobApplications.Add(jobApplications);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.AppID = new SelectList(db.applicants, "AppID", "RSAID", jobApplications.AppID);
            ViewBag.JobID = new SelectList(db.jobs, "JobID", "JobType", jobApplications.JobID);
            return View(jobApplications);
        }

        // GET: JobApplications/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            JobApplications jobApplications = db.JobApplications.Find(id);
            if (jobApplications == null)
            {
                return HttpNotFound();
            }
            ViewBag.AppID = new SelectList(db.applicants, "AppID", "RSAID", jobApplications.AppID);
            ViewBag.JobID = new SelectList(db.jobs, "JobID", "JobType", jobApplications.JobID);
            return View(jobApplications);
        }

        // POST: JobApplications/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "JobAppID,JobID,AppID,AppliedDate,Status,Resume")] JobApplications jobApplications)
        {
            if (ModelState.IsValid)
            {
                db.Entry(jobApplications).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AppID = new SelectList(db.applicants, "AppID", "RSAID", jobApplications.AppID);
            ViewBag.JobID = new SelectList(db.jobs, "JobID", "JobType", jobApplications.JobID);
            return View(jobApplications);
        }

        // GET: JobApplications/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            JobApplications jobApplications = db.JobApplications.Find(id);
            if (jobApplications == null)
            {
                return HttpNotFound();
            }
            return View(jobApplications);
        }

        // POST: JobApplications/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            JobApplications jobApplications = db.JobApplications.Find(id);
            db.JobApplications.Remove(jobApplications);
            db.SaveChanges();
            return RedirectToAction("MyApplications", new {id = jobApplications.AppID });
        }

        public ActionResult Apply(int id)
        {
            JobApplications Applyjob = new JobApplications();
            Applyjob.JobID = id;
            return PartialView(Applyjob);
        }

        [HttpPost]
        public ActionResult Apply(JobApplications jobApplication, HttpPostedFileBase file)
        {
            var checkaaplicant = db.applicants.Where(x => x.RSAID == jobApplication.ID).FirstOrDefault();

            if (checkaaplicant == null)
            {
                return RedirectToAction("NotFound");
            }

            JobApplications newJobApp = new JobApplications();
            Session["myapp"] = checkaaplicant.AppID;
            newJobApp.AppID = checkaaplicant.AppID;
            newJobApp.AppliedDate = DateTime.Now;
            newJobApp.JobID = jobApplication.JobID;
            newJobApp.Status = "Pending";

            var supportedPdfTypes = new[] { "pdf" };
            var PdfFileSize = 2000000;//2MB MAX
            if (file != null)
            {

                if (file.ContentLength > (PdfFileSize))
                {
                    ViewBag.FileError = "File Size should be less than 2MB";
                    return PartialView();
                }
                else if (!supportedPdfTypes.Contains(System.IO.Path.GetExtension(file.FileName).Substring(1)))
                {
                    ViewBag.FileError = "Invalid File Format";
                }
                else
                {

                    Stream str = file.InputStream;
                    BinaryReader Br = new BinaryReader(str);
                    Byte[] PDFfile = Br.ReadBytes((Int32)str.Length);
                    newJobApp.Resume = PDFfile;
                    db.JobApplications.Add(newJobApp);
                    db.SaveChanges();

                }
            }
           
            return RedirectToAction ("SuccessfulApp");
        }


        public ActionResult SuccessfulApp()
        {
            return View();
        }

        public ActionResult NotFound()
        {
            return View();
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
