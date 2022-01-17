using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BusinesssTrinitySP01.Models;

namespace BusinesssTrinitySP01.Controllers
{
    public class PackagesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Packages
        public ActionResult Index()
        {
            var packages = db.packages.Include(p => p.ClientProfile);
            return View(packages.ToList());
        }

        // GET: Packages/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Package package = db.packages.Find(id);
            if (package == null)
            {
                return HttpNotFound();
            }
            return View(package);
        }

        // GET: Packages/Create
        [HttpGet]
        public ActionResult Create(int id)
        {
            Package package = new Package();
            var GetUser = db.clientProfiles.Where(x => x.Email == User.Identity.Name).Select(x => x.Email).FirstOrDefault();
            package.Email = GetUser;
            package.OrderID = id;
            package.Name = "My Design Package";
            return PartialView(package);
        }

        // POST: Packages/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "PckId,Name,DateCreated,Email,CartID,ThemeID,OrderID")] Package package)
        {
            if (ModelState.IsValid)
            {
                package.DateCreated = DateTime.Now.Date;

                db.packages.Add(package);
                db.SaveChanges(); 
                //return RedirectToAction("GetOrder", "DecorRentals", new { PckID = package.PckId });
                return RedirectToAction("SelectedOrder", "DecorRentals", new { PckID = package.PckId });
            }

            return View(package);
        }

        // GET: Packages/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Package package = db.packages.Find(id);
            if (package == null)
            {
                return HttpNotFound();
            }
            return View(package);
        }

        // POST: Packages/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PckId,Name,DateCreated,Email,CartID,ThemeID,OrderID")] Package package)
        {
            if (ModelState.IsValid)
            {
                db.Entry(package).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(package);
        }

        // GET: Packages/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Package package = db.packages.Find(id);
            if (package == null)
            {
                return HttpNotFound();
            }
            return View(package);
        }

        // POST: Packages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Package package = db.packages.Find(id);
            db.packages.Remove(package);
            db.SaveChanges();
            return RedirectToAction("Index");
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
