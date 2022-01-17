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
    public class SupAddressesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: SupAddresses
        public ActionResult Index()
        {
            var supAddresses = db.supAddresses.Include(s => s.supplier);
            return View(supAddresses.ToList());
        }

        // GET: SupAddresses/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SupAddress supAddress = db.supAddresses.Find(id);
            if (supAddress == null)
            {
                return HttpNotFound();
            }
            return View(supAddress);
        }

        // GET: SupAddresses/Create
        public ActionResult Create()
        {
            ViewBag.SuppID = new SelectList(db.suppliers, "SuppID", "SuppName");
            return View();
        }

        // POST: SupAddresses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "SpID,Streetno,Suburb,City,Province,PostalCode,SuppID")] SupAddress supAddress)
        {
            if (ModelState.IsValid)
            {
                db.supAddresses.Add(supAddress);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.SuppID = new SelectList(db.suppliers, "SuppID", "SuppName", supAddress.SuppID);
            return View(supAddress);
        }

        // GET: SupAddresses/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SupAddress supAddress = db.supAddresses.Find(id);
            if (supAddress == null)
            {
                return HttpNotFound();
            }
            ViewBag.SuppID = new SelectList(db.suppliers, "SuppID", "SuppName", supAddress.SuppID);
            return View(supAddress);
        }

        // POST: SupAddresses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "SpID,Streetno,Suburb,City,Province,PostalCode,SuppID")] SupAddress supAddress)
        {
            if (ModelState.IsValid)
            {
                db.Entry(supAddress).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.SuppID = new SelectList(db.suppliers, "SuppID", "SuppName", supAddress.SuppID);
            return View(supAddress);
        }

        // GET: SupAddresses/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SupAddress supAddress = db.supAddresses.Find(id);
            if (supAddress == null)
            {
                return HttpNotFound();
            }
            return View(supAddress);
        }

        // POST: SupAddresses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            SupAddress supAddress = db.supAddresses.Find(id);
            db.supAddresses.Remove(supAddress);
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
